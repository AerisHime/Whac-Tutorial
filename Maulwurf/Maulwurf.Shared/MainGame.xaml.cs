using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Maulwurf
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainGame : Page
    {
        private bool isRunning;
        private class Player
        {
            public int score { get; set; }
            public int lives { get; set; }

        }
        private class Mole
        {
            public int timer { get; set; }
            public Image img { get; set; }

        }
        private Player player;
        private Mole[] moles = new Mole[9];
        private DispatcherTimer countdownTimer;
        private int counter;
        private int timeToNextMole;
        private Random rnd = new Random((int)DateTime.Now.Ticks);

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);
            InitGame();
        }
        private void InitGame()
        {
            isRunning = true;
            counter = 3;
            player = new Player { lives = 4, score = 0 };
            lives.Count = player.lives;
            txtCountdown.Text = counter.ToString();
            txtScore.Text = player.score.ToString();
            for (int i = 0; i < moles.Length; i++)
            {
                moles[i] = new Mole { timer = -1, img = null };
            }

            countdownTimer = new DispatcherTimer();
            countdownTimer.Interval = new TimeSpan(0, 0, 1);
            countdownTimer.Tick += countdownTimer_Tick;
            countdownTimer.Start();

        }

        void countdownTimer_Tick(object sender, object e)
        {
            
            counter -= 1;
            txtCountdown.Text = counter.ToString();
            if (counter <= -1)
            {
                txtCountdown.Visibility = Visibility.Collapsed;
                countdownTimer.Stop();
                RunGame();
            }
        }

        private async void RunGame()
        {
            timeToNextMole = 10;
            while (isRunning)
            {
                await Task.Delay(100);
                CheckAllMoles();
                timeToNextMole -= 1;
                if (timeToNextMole <= 0)
                {
                    InsertNewMole();
                }
                txtScore.Text = player.score.ToString();
                lives.Count = player.lives;

            }
        }

        private void InsertNewMole()
        {
            var pos = rnd.Next(0, 8);
            if (moles[pos].timer < 0)
            {
                moles[pos].timer = rnd.Next(10, 30);
                timeToNextMole = rnd.Next(10, 40);
                var row = pos / 3;
                var column = pos % 3;
                moles[pos].img = new Image();
                gameField.Children.Add(moles[pos].img);
                moles[pos].img.SetValue(Grid.ColumnProperty, column);
                moles[pos].img.SetValue(Grid.RowProperty, row);
                moles[pos].img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Target.png"));
                moles[pos].img.Tag = moles[pos];
                moles[pos].img.Tapped += img_Tapped;
            }
        }

        void img_Tapped(object sender, TappedRoutedEventArgs e)
        {
            var img = sender as Image;
            if (!isRunning)
            {
                return;
            }
            if (img == null)
            {
                return;
            }
            var tempmole = img.Tag as Mole;
            if (tempmole == null)
            {

                return;
            }
            tempmole.timer = -1;
            gameField.Children.Remove(tempmole.img);
            tempmole.img = null;
            player.score += 50;

        }

        private void CheckAllMoles()
        {
            for (int i = 0; i < moles.Length; i++)
            {
                if (moles[i].timer >= 0)
                {
                    moles[i].timer -= 1;
                }
                else
                {
                    if (moles[i].img != null)
                    {
                        gameField.Children.Remove(moles[i].img);
                        moles[i].img = null;
                        player.lives -= 1;
                        if (player.lives <= 0)
                        {
                            GameOver();
                            break;
                        }
                    }
                }
            }
        }

        private void GameOver()
        {
            isRunning = false;
            txtCountdown.Text = Encoding.Unicode.GetString(new byte[] { 0x39, 0x26 }, 0, 2);
            txtCountdown.Visibility = Visibility.Visible;

        }

        public MainGame()
        {
            this.InitializeComponent();
        }

        private void txtCountdown_Tapped(object sender, TappedRoutedEventArgs e)
        {
            if (!isRunning)
            {
                InitGame();
            }
        }
    }
}
