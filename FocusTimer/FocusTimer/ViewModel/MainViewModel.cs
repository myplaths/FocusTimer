using BoxesCalculator.Commands;
using FocusTimer.Model;
using Plugin.SimpleAudioPlayer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text;
using System.Windows.Input;
using Xamarin.Forms;

namespace FocusTimer.ViewModel
{
    public class MainViewModel : INotifyPropertyChanged
    {
        #region propertychanged
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        #region colors
        string defaultBackGroundColor = "#2c3e50";
        string defaultButtonColor = "#34495e";
        string redColor = "#e74c3c";
        string greenColor = "#1abc9c";
        #endregion

        #region commands
        public DelegateCommand RepeatCommand { get; set; }
        public DelegateCommand SoundCommand { get; set; }
        public DelegateCommand StartCommand { get; set; }
        public DelegateCommand StopCommand { get; set; }
        #endregion

        #region properties
        private int counter;
        bool repeatTimer;
        bool stoptimer = false;
        bool isRepeated = false;
        int countdownTime;
        bool isBackGroundChanged = false;
        bool hasSound;

        private ISimpleAudioPlayer _simpleAudioPlayer;   
     

        private bool btnRepeatIsEnabled;

        public bool BtnRepeatIsEnabled
        {
            get { return btnRepeatIsEnabled; }
            set
            {
                btnRepeatIsEnabled = value;
                OnPropertyChanged(nameof(BtnRepeatIsEnabled));
            }
        }
        
        private bool btnStartIsEnabled;

        public bool BtnStartIsEnabled
        {
            get { return btnStartIsEnabled; }
            set
            {
                btnStartIsEnabled = value;
                OnPropertyChanged(nameof(BtnStartIsEnabled));
            }
        }

        private string btnRepeatText;

        public string BtnRepeatText
        {
            get { return btnRepeatText; }
            set
            {
                btnRepeatText = value;
                OnPropertyChanged(nameof(BtnRepeatText));
            }
        }

        private string btnSoundText;

        public string BtnSoundText
        {
            get { return btnSoundText; }
            set { btnSoundText = value;
                OnPropertyChanged(nameof(BtnSoundText));
            }
        }

        private string timerText;

        public string TimerText
        {
            get { return timerText; }
            set { timerText = value;
                OnPropertyChanged(nameof(TimerText));
            }
        }

        public ObservableCollection<PickerModel> PickerList { get; set; }
        public PickerModel SelectedTime { get; set; }




        private Color btnRepeatBackGroundColor;
    public Color BtnRepeatBackGroundColor
        {
            get { return btnRepeatBackGroundColor; }
            set
            {
                if (value == btnRepeatBackGroundColor)
                    return;

                btnRepeatBackGroundColor = value;
                OnPropertyChanged(nameof(BtnRepeatBackGroundColor));
            }
        }

        private Color btnSoundBackgroundColor;
        public Color BtnSoundBackgroundColor
        {
            get { return btnSoundBackgroundColor; }
            set
            {
                if (value == btnSoundBackgroundColor)
                    return;

                btnSoundBackgroundColor = value;
                OnPropertyChanged(nameof(BtnSoundBackgroundColor));
            }
        }

        private Color btnStacklayoutMainColor;
        public Color BtnStacklayoutMainColor
        {
            get { return btnStacklayoutMainColor; }
            set
            {
                if (value == btnStacklayoutMainColor)
                    return;

                btnStacklayoutMainColor = value;
                OnPropertyChanged(nameof(BtnStacklayoutMainColor));
            }
        }
        #endregion
       
        public MainViewModel()
        {
            InitializePickerList();
            InitializeAudio();
            InitializeUIColors();
            InitializeDefaultValues();
            InitializeCommands();
            SelectedTime = PickerList[0];
        }

        private void InitializeDefaultValues()
        {
            BtnRepeatText = "Yes";
            BtnSoundText = "No";
            TimerText = "00";
            repeatTimer = true;
            BtnRepeatIsEnabled = true;
        }

        private void InitializeCommands()
        {
            RepeatCommand = new DelegateCommand(Repeat);
            SoundCommand = new DelegateCommand(BtnSound);
            StartCommand = new DelegateCommand(BtnStart);
            StopCommand = new DelegateCommand(BtnStop);
        }

        private void InitializeUIColors()
        {
            BtnRepeatBackGroundColor = Color.FromHex(defaultButtonColor);
            BtnSoundBackgroundColor = Color.FromHex(defaultButtonColor);
            BtnStacklayoutMainColor = Color.FromHex(defaultBackGroundColor);
        }

        private void InitializePickerList()
        {
            PickerList = new ObservableCollection<PickerModel>()
            {
                new PickerModel{value = 1},
                new PickerModel{value = 2},
                new PickerModel{value = 3},
                new PickerModel{value = 4},
                new PickerModel{value = 5},
                new PickerModel{value = 6},
                new PickerModel{value = 7},
                new PickerModel{value = 8},
                new PickerModel{value = 9},
                new PickerModel{value = 10}
            };
        }

        void InitializeAudio()
        {
            _simpleAudioPlayer = CrossSimpleAudioPlayer.CreateSimpleAudioPlayer();
            Stream beepStream = GetType().Assembly.GetManifestResourceStream("FocusTimer.beep.mp3");
            bool isSuccess = _simpleAudioPlayer.Load(beepStream);
        }

        private void BtnStop()
        {
            stoptimer = true;
            isRepeated = false;
            BtnStartIsEnabled = true;
        }

        private void BtnStart()
        {
            TimerStart();
        }

        private void TimerStart()
        {
            stoptimer = false;
            countdownTime = ConvertSecondsToMinute(SelectedTime.value);
            counter = countdownTime;
            if (!stoptimer)
            {
                if (repeatTimer)
                {
                    RunRepeatedTimer();
                }
                else
                {
                    RunTimerOneTime();
                }
            }
        }

        void Test()
        {
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                counter--;
                TimerText = counter.ToString();
                return true;
            });
        }

        private void RunTimerOneTime()
        {
            bool timerOffBool = true;

            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CountDownAndRevertsBackgroundColor();
                if (counter == 0)
                {
                    Notification();
                    counter = countdownTime;
                    timerOffBool = false;
                }
                return timerOffBool;
            });
        }

        private void CountDownAndRevertsBackgroundColor()
        {
            BtnStacklayoutMainColor = Color.FromHex(defaultBackGroundColor);
            counter--;
            TimerText = counter.ToString();
        }

        void Notification()
        {
            BtnStacklayoutMainColor = Color.FromHex(greenColor);
            if (hasSound)
            {
                _simpleAudioPlayer.Play();
            }
        }



        private void RunRepeatedTimer()
        {
            isRepeated = !stoptimer;
            Device.StartTimer(TimeSpan.FromSeconds(1), () =>
            {
                CountDownAndRevertsBackgroundColor();
                if (counter == 0)
                {
                    Notification();
                    counter = countdownTime;
                }
                return isRepeated;
            });
        }

        int ConvertSecondsToMinute(int number)
        {
            if (number == 0)
            {
                number = 5;
            }
            if (number == 1)
            {
                number = 60;
            }
            if (number == 2)
            {
                number = 120;
            }
            if (number == 3)
            {
                number = 180;
            }
            if (number == 4)
            {
                number = 240;
            }
            if (number == 5)
            {
                number = 300;
            }
            if (number == 6)
            {
                number = 360;
            }
            if (number == 7)
            {
                number = 420;
            }
            if (number == 8)
            {
                number = 480;
            }
            if (number == 9)
            {
                number = 540;
            }
            if (number == 10)
            {
                number = 600;
            }
            return number;
        }

        private void BtnSound()
        {
            hasSound = !hasSound;
            isBackGroundChanged = !isBackGroundChanged;

            UpdateSoundText();

            UpdateBtnSoundBackgroundColor();

        }

        private void UpdateBtnSoundBackgroundColor()
        {
            if (isBackGroundChanged)
            {
                BtnSoundBackgroundColor = Color.FromHex(redColor);
            }
            else
            {
                BtnSoundBackgroundColor = Color.FromHex(defaultButtonColor);
            }
        }

        private void UpdateSoundText()
        {
            if (hasSound)
            {
                BtnSoundText = "Yes";
            }
            else
            {
                BtnSoundText = "No";
            }
        }

        private void Repeat()
        {
            BtnStartIsEnabled = false;
            repeatTimer = !repeatTimer;
            if (repeatTimer)
            {
                BtnRepeatBackGroundColor = Color.FromHex(defaultButtonColor);
                BtnRepeatText = "Yes";
            }
            else
            {
                BtnRepeatBackGroundColor = Color.FromHex(redColor);
                BtnRepeatText = "No";
            }
        }
    }
}
