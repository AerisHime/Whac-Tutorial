using System;
using System.Text;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
using Microsoft.WindowsAzure.MobileServices;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Whac_A_Mole
{

    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainGame : Page
    {
        private MobileServiceCollection<GameRanks, GameRanks> items;
        private IMobileServiceTable<GameRanks> RankingTable = App.MobileService.GetTable<GameRanks>();
        
        private bool isRunning;
        /*
         * Spielfigurendefinition
         * */
        class Player
        {
            public int Score { get; set; }
            public int Lives { get; set; }

        }
        class Mole
        {
            public int Timer { get; set; }
            public Image Img { get; set; }
        }

        private Player player;
        private Mole[] moles = new Mole[9];

        //Zeitzählervariablen
        private int timeToNextMole;
        private int counter;

        private DispatcherTimer countdownTimer;

        private Random rnd = new Random((int)DateTime.Now.Ticks);

        public MainGame()
        {

            this.InitializeComponent();
            RefreshItems();
            InitGame();
        }

        private async Task RefreshItems()
        {
            items = await RankingTable.ToCollectionAsync();
        }

        private void InitGame()
        {
            isRunning = true;
            player = new Player { Lives = 4, Score = 0 };
            counter = 3;
            lives.Count = player.Lives;
            txtScore.Text = player.Score.ToString();
            txtCountdown.Text = counter.ToString();
            for (int i = 0; i < moles.Length; i++)
            {
                moles[i] = new Mole { Timer = -1, Img = null };
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
                StartGame();
            }
        }

        private async void StartGame()
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
                txtScore.Text = player.Score.ToString();
                lives.Count = player.Lives;

                
            }
        }

        private async Task SendScore (GameRanks ranking)
        {
            await RankingTable.InsertAsync(ranking);
            items.Add(ranking);
        }
        private void InsertNewMole()
        {
            var pos = rnd.Next(0, 9);
            if (moles[pos].Timer < 0)
            {
                moles[pos].Timer = rnd.Next(10, 30);
                timeToNextMole = rnd.Next(10, 40);
                var row = pos / 3;
                var column = pos % 3;
                moles[pos].Img = new Image();
                gameField.Children.Add(moles[pos].Img);
                moles[pos].Img.SetValue(Grid.ColumnProperty, column);
                moles[pos].Img.SetValue(Grid.RowProperty, row);
                moles[pos].Img.Source = new BitmapImage(new Uri("ms-appx:///Assets/Target.png"));
                moles[pos].Img.Tag = moles[pos];
                moles[pos].Img.Tapped += Img_Tapped;
            }
        }

        void Img_Tapped(object sender, TappedRoutedEventArgs e)
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
            var tempMole = img.Tag as Mole;
            if (tempMole == null)
            {
                return;
            }
            tempMole.Timer = -1;
            gameField.Children.Remove(tempMole.Img);
            tempMole.Img = null;
            player.Score += 50;

        }


        private void CheckAllMoles()
        {
            for (int i = 0; i < moles.Length; i++)
            {
                if (moles[i].Timer >= 0)
                {
                    moles[i].Timer -= 1;
                }
                else
                {
                    if (moles[i].Img != null)
                    {
                        gameField.Children.Remove(moles[i].Img);
                        moles[i].Img = null;
                        player.Lives -= 1;
                        if (player.Lives <= 0)
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
            var gameOverRanking = new GameRanks { username = "Spieler A ", score = player.Score, superHighScore = 1000 };
            SendScore(gameOverRanking);
            App.MobileService.LoginAsync(MobileServiceAuthenticationProvider.Twitter);

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
