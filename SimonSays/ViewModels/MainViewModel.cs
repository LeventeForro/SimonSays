using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using System.IO;
using NAudio.Wave;
using SimonSays.Model;

namespace SimonSays.ViewModels
{
    public class MainViewModel : ViewModelBase
    {
        private GameModel _gameModel;
        private readonly List<string> _colorSequence = new();
        private int _currentIndex;
        private readonly Random _random = new();
        private readonly Dictionary<string, string> _buttonColors;
        private bool _gameStarted = false;
        private bool isProcessing = false;

        private WaveOutEvent? _outputDevice; // A lejátszó
        private AudioFileReader? _audioFileReader; // A hang fájl olvasó

        public Dictionary<string, string> ButtonColors
        {
            get => _buttonColors;
        }

        private string _levelText = "Szint: 1";
        public string LevelText
        {
            get => _levelText;
            set => SetProperty(ref _levelText, value);
        }

        public ICommand StartCommand { get; }
        public ICommand ColorClickCommand { get; }

        public MainViewModel()
        {
            _gameModel = new GameModel();

            _buttonColors = new Dictionary<string, string>
            {
                { "Red", "Red" },
                { "Blue", "Blue" },
                { "Green", "Green" },
                { "Yellow", "Yellow" },
                { "Purple", "Purple" },
                { "Fuchsia", "Fuchsia" },
                { "DarkKhaki", "DarkKhaki" },
                { "Turquoise", "Turquoise" },
                { "Coral", "Coral" }
            };

            StartCommand = new RelayCommand(StartGame);
            ColorClickCommand = new RelayCommand<object>(OnColorClicked);
        }

        private async void StartGame()
        {
            if (_gameStarted) return;

            _gameStarted = true;
            _gameModel.ResetGame();  // Reseteljuk a játékot a modellben
            _currentIndex = 0;
            LevelText = "Szint: 1";
            await AddNewColorAndDisplaySequence();
        }

        private async Task AddNewColorAndDisplaySequence()
        {
            isProcessing = true;

            // Hozzáadunk egy új színt a modellhez
            _gameModel.AddRandomStep();

            _currentIndex = 0;

            // Megmutatjuk a szekvenciát a felhasználónak
            foreach (var color in _gameModel.Sequence)
            {
                await HighlightButton(color);
                await Task.Delay(300);  // Kiemelés időtartama
                ResetButton(color);
                await Task.Delay(100);  // Rövid szünet színek között
            }

            isProcessing = false;
        }


        private async Task HighlightButton(string color)
        {
            UpdateButtonColor(color, "White");
            await PlaySound(color);
            await Task.Delay(500); // Villogás időtartama
            ResetButton(color);
        }

        private void ResetButton(string color)
        {
            UpdateButtonColor(color, color); // Eredeti szín visszaállítása
        }


        private string _redButtonColor = "Red";
        public string RedButtonColor
        {
            get => _redButtonColor;
            set => SetProperty(ref _redButtonColor, value);
        }

        private string _blueButtonColor = "Blue";
        public string BlueButtonColor
        {
            get => _blueButtonColor;
            set => SetProperty(ref _blueButtonColor, value);
        }

        private string _greenButtonColor = "Green";
        public string GreenButtonColor
        {
            get => _greenButtonColor;
            set => SetProperty(ref _greenButtonColor, value);
        }

        private string _yellowButtonColor = "Yellow";
        public string YellowButtonColor
        {
            get => _yellowButtonColor;
            set => SetProperty(ref _yellowButtonColor, value);
        }

        private string _purpleButtonColor = "Purple";
        public string PurpleButtonColor
        {
            get => _purpleButtonColor;
            set => SetProperty(ref _purpleButtonColor, value);
        }

        private string _fuchsiaButtonColor = "Fuchsia";
        public string FuchsiaButtonColor
        {
            get => _fuchsiaButtonColor;
            set => SetProperty(ref _fuchsiaButtonColor, value);
        }

        private string _darkKhakiButtonColor = "DarkKhaki";
        public string DarkKhakiButtonColor
        {
            get => _darkKhakiButtonColor;
            set => SetProperty(ref _darkKhakiButtonColor, value);
        }

        private string _turquoiseButtonColor = "Turquoise";
        public string TurquoiseButtonColor
        {
            get => _turquoiseButtonColor;
            set => SetProperty(ref _turquoiseButtonColor, value);
        }

        private string _coralButtonColor = "Coral";
        public string CoralButtonColor
        {
            get => _coralButtonColor;
            set => SetProperty(ref _coralButtonColor, value);
        }


        private void UpdateButtonColor(string color, string newColor)
        {
            switch (color)
            {
                case "Red":
                    RedButtonColor = newColor;
                    break;
                case "Blue":
                    BlueButtonColor = newColor;
                    break;
                case "Green":
                    GreenButtonColor = newColor;
                    break;
                case "Yellow":
                    YellowButtonColor = newColor;
                    break;
                case "Purple":
                    PurpleButtonColor = newColor;
                    break;
                case "Fuchsia":
                    FuchsiaButtonColor = newColor;
                    break;
                case "DarkKhaki":
                    DarkKhakiButtonColor = newColor;
                    break;
                case "Turquoise":
                    TurquoiseButtonColor = newColor;
                    break;
                case "Coral":
                    CoralButtonColor = newColor;
                    break;
            }
        }



        private async void OnColorClicked(object? parameter)
        {
            if (!_gameStarted || isProcessing) return;

            if (parameter is not string clickedColor)
                return;

            // Itt lejátszuk a hangot a szín alapján
            await PlaySound(clickedColor);

            // Ellenőrizzük, hogy a felhasználó helyesen választotta-e a színt a modellben
            if (_gameModel.CheckStep(_currentIndex, clickedColor))
            {
                _currentIndex++;

                if (_currentIndex >= _gameModel.Sequence.Count)
                {
                    LevelText = $"Szint: {_gameModel.Sequence.Count + 1}";
                    await Task.Delay(1000);
                    await AddNewColorAndDisplaySequence();
                }
            }
            else
            {
                LevelText = $"Game Over! Elért szint: {_gameModel.Sequence.Count}";
                _gameStarted = false;
            }
        }




        private async Task PlaySound(string color)
        {
            // Állítsuk le a régi lejátszást, ha még fut
            StopSound();

            // Hang fájl elérési útvonalának összeállítása
            string soundFile = Path.Combine(AppContext.BaseDirectory, "Assets", "Sounds", $"{color.ToLower()}.wav");

            if (File.Exists(soundFile))
            {
                // Nyissuk meg a fájlt
                _audioFileReader = new AudioFileReader(soundFile);
                _outputDevice = new WaveOutEvent();
                _outputDevice.Init(_audioFileReader);
                _outputDevice.Play();
            }
        }


        private void StopSound()
        {
            if (_outputDevice != null && _outputDevice.PlaybackState == PlaybackState.Playing)
            {
                _outputDevice.Stop();
                _audioFileReader?.Dispose();
                _outputDevice?.Dispose();
            }
        }

    }
}
