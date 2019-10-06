using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace _8PuzzelGame
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                // icons
                icons = new ImageName();
                this.DataContext = icons;
                this.Icon = new BitmapImage(new Uri(icons.ApplicationIcon[0]));

                // game
                game = new Game();

                // timer
                timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += Timer_Tick;

                TimerTextBlock.Text = "Timer " + MaxMinutes.ToString().PadLeft(2, '0') + ":" + "0".PadLeft(2, '0');
                countSecond = 0;

                // images & board
                NewImagelistView();
                NewBoardListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        #region Image and board
        // list of images and board
        ImageName icons;
        ImageName images;
        BindingList<MyInt> listValue;


        // get all images in a default folder to show a list of available images
        // [catch exception in calling function]
        private void NewImagelistView()
        {
            // get all images in folder 'Images'
            images = new ImageName(@"Images\");
            ImageListView.ItemsSource = images.Images;
            ImageListView.SelectedIndex = 0;

        }

        // create new board to contain tiles of image
        // reset all related states
        // [catch exception in calling function]
        private void NewBoardListView()
        {
            // crop image
            string filename = ImageListView.SelectedItem as string;
            GenerateTiles(filename, ref ImageTiles);

            // reset state
            isShowResult = false;
            TerminateGameAndRelatedThreads();

            game.Reset();

            StopTimer();
            TimerTextBlock.Text = "Timer " + MaxMinutes.ToString().PadLeft(2, '0') + ":" + "0".PadLeft(2, '0');
            countSecond = 0;

            isBoardEnabled = false;


            // set value for board
            Board board = game.GetCurrentBoard();
            listValue = new BindingList<MyInt>();
            for (int i = 0; i < Game.NumberOfTiles; i++)
            {
                listValue.Add(new MyInt() { Value = board.GetAt(i) });
            }
            BoardListView.ItemsSource = listValue;

        }

        
        // update board containing tiles of images
        // reset value of tiles
        private void UpdateBoardListView()
        {
            string filename = ImageListView.SelectedItem as string;
            GenerateTiles(filename, ref ImageTiles);

            // set value for board
            Board board = game.GetCurrentBoard();
            listValue = new BindingList<MyInt>();

            for (int i = 0; i < Game.NumberOfTiles; i++)
            {
                listValue.Add(new MyInt() { Value = board.GetAt(i) });
            }
            BoardListView.ItemsSource = listValue;

        }

        // add a new image into list of images
        // set focus to that new image
        private void UpdateImageListView(string newImageFilename)
        {
            // add image
            images.Images.Add(newImageFilename);

            // set selected
            var item = ImageListView.Items[ImageListView.Items.Count - 1];
            ImageListView.SelectedItem = item;
            ImageListView.ScrollIntoView(item);


            // set focus
            var container = ImageListView.ItemContainerGenerator.ContainerFromItem(item) as ListBoxItem;
            if (container != null)
            {
                container.Focus();
            }
            

        }

        
        // shuffle positions of tiles of a image contained in board
        private void ShuffleButton_Click(object sender, RoutedEventArgs e)
        {
            game.Shuffle();

            Board board = game.GetCurrentBoard();
            for (int i = 0; i < Game.NumberOfTiles; i++)
            {
                listValue[i].Value = board.GetAt(i);
            }
        }

        // event user click a image in list of images
        private void ImageListView_Click(object sender, MouseButtonEventArgs e)
        {
            try
            {
                NewBoardListView();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        // event user click on browse button
        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;
                bool isSuccess = true;

                try
                {
                    // show new image in listview
                    UpdateImageListView(filename);
        

                    // set board to new image
                    NewBoardListView();
     
                }
                catch (Exception ex)
                {
                    isSuccess = false;
                    MessageBox.Show(ex.Message);
                }
            }
        }


        // utility function to force ui update
        private void AllowUIToUpdate()
        {
            DispatcherFrame frame = new DispatcherFrame();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.Render, new DispatcherOperationCallback(delegate (object parameter)
            {
                frame.Continue = false;
                return null;
            }), null);

            Dispatcher.PushFrame(frame);
            //EDIT:
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
                                          new Action(delegate { }));
        }

        public static CroppedBitmap[] ImageTiles = null;

        // crop image into tiles
        private void GenerateTiles(string inputImage, ref CroppedBitmap[] tiles)
        {
            // source
            BitmapImage source = new BitmapImage();
            source.BeginInit();
            source.UriSource = new Uri(inputImage);
            source.CacheOption = BitmapCacheOption.OnLoad;
            source.EndInit();

            double subWidth = source.PixelWidth / Game.Columns;
            double subHeight = source.PixelHeight / Game.Rows;


            // crop
            tiles = new CroppedBitmap[Game.NumberOfTiles];
            int count = 0;
            for (int i = 0; i < Game.Rows; i++)
            {
                for (int j = 0; j < Game.Columns; j++)
                {
                    tiles[count++] = new CroppedBitmap(source,
                                                new Int32Rect((int)(j * subWidth), (int)(i * subHeight),
                                                (int)subWidth, (int)subHeight));
                }
            }


        }


        #endregion


        #region game event and timer
        bool isShowResult = true;

        // game event and timer
        private void PlayGameButton_Click(object sender, RoutedEventArgs e)
        {
            if (isGameStopped == true)
            {
                // create threads
                isBoardEnabled = true;
                isGameStopped = false;
                isShowResult = true;
                CreateGameAndRelatedThreads();

                // start timer
                //countSecond = 0;
                StartTimer();
            }

            else
            {
                MessageBox.Show("You are playing");
            }
        }

        private void CreateGameAndRelatedThreads()
        {
            
            // create game thread
            gameThread = new Thread(GameProcess);
            gameSemaphore = new Semaphore(0, 1);
            gameThread.Start();


            // create update UI thread
            uiUpdateThread = new Thread(UIUpdateProcess);
            uiUpdateSemaphore = new Semaphore(0, 1);
            uiUpdateThread.Start();
        }

        private void TerminateGameAndRelatedThreads()
        {
            // terminate threads
            if (isGameStopped == false && gameThread != null && gameSemaphore != null)
            {
                //MessageBox.Show("terminated thread!");
                isGameStopped = true;

                gameSemaphore.Release();

                
            }

        }

        // timer
        DispatcherTimer timer;
        int countSecond = 0;
        int MaxMinutes = 3;

        private void StartTimer()
        {
            timer.Start();
        }

        private void StopTimer()
        {
            if (timer.IsEnabled)
            {
                timer.Stop();
      
            }
        }

        private void UpdateTimerUI()
        {

            int min = (MaxMinutes - 1) - countSecond / 60;
            int sec = 59 - countSecond % 60;

            TimerTextBlock.Text = "Timer " + min.ToString().PadLeft(2, '0') + ":" + sec.ToString().PadLeft(2, '0');

        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            UpdateTimerUI();

            countSecond++;

            if (countSecond >= MaxMinutes * 60)
            {
                // stop game and show result
                isShowResult = true;
                TerminateGameAndRelatedThreads();
            }
        }

        bool isBoardEnabled = false;

        #endregion


        #region drag and move
        bool isMouseEnter = false;
        bool isDragging = false;
        Image tempImage = null;
        ListViewItem item = null;
        int imageWidth;
        int imageHeight;
        int lastValue;
        int lastIndex;
        int newIndex;

        // drag & move
        private void BoardListView_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (isBoardEnabled == false)
            {
                MessageBox.Show("Please click shuffle then click play!");
                return;
            }


            // set selected item
            BoardListView.SelectedItems.Clear();
            item = sender as ListViewItem;
            if (item != null)
            {
                item.IsSelected = true;
                BoardListView.SelectedItem = item;
            }

            // turn flag on
            isDragging = true;
            var position = e.GetPosition(GameGrid);

            // create image to drag
            CroppedBitmap bitmap = IntValueToImageNameConverter.Instance.Convert(
                (Object)(BoardListView.SelectedItem as MyInt).Value, null, null, null) as CroppedBitmap;
            tempImage = new Image();
            tempImage.Source = bitmap;


            Thickness margin = new Thickness();
            imageWidth = (int)item.ActualWidth - 5;
            imageHeight = (int)item.ActualHeight - 5;
            double totalWidth = GameGrid.ActualWidth;
            double totalHeight = GameGrid.ActualHeight;

            margin.Left = position.X;
            margin.Top = position.Y;
            margin.Right = totalWidth - (position.X + imageWidth);
            margin.Bottom = totalHeight - (position.Y + imageHeight);

            tempImage.Margin = margin;
            tempImage.Stretch = Stretch.Fill;

            // save last index, value
            lastIndex = BoardListView.SelectedIndex;
            lastValue = listValue[lastIndex].Value;

            // set tiles to empty
            listValue[lastIndex].Value = Game.EmptyValue;

            // add dragging image to grid
            GameGrid.Children.Add(tempImage);

        }

        private void BoardListView_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {

                var position = e.GetPosition(GameGrid);

                int imageWidth = (int)item.ActualWidth - 5;
                int imageHeight = (int)item.ActualHeight - 5;
                double totalWidth = GameGrid.ActualWidth;
                double totalHeight = GameGrid.ActualHeight;

                // set position of temp image
                Thickness margin = tempImage.Margin;
                margin.Left = position.X;
                margin.Top = position.Y;
                margin.Right = totalWidth - (position.X + imageWidth);
                margin.Bottom = totalHeight - (position.Y + imageHeight);
                tempImage.Margin = margin;

            }
        }

        private void BoardListView_MouseUp(object sender, MouseEventArgs e)
        {
            if (isDragging)
            {
                // set flag
                isDragging = false;

                // remove temp image
                tempImage.Source = null;
                GameGrid.Children.Remove(tempImage);


                // set position of image in board
                if (isMouseEnter)
                {
                    //MessageBox.Show($"{i} {j}");
                    listValue[newIndex].Value = lastValue;

                    // update game process
                    gameSemaphore.Release();
                }
                else
                {
                    MessageBox.Show("Move not valid");
                    newIndex = -1;
                    listValue[lastIndex].Value = lastValue;
                }

            }
        }

        private void BoardListViewItem_MouseMove(object sender, MouseEventArgs e)
        {
            //MessageBox.Show(sender.ToString());
            var itemMoveOn = sender as ListBoxItem;
            newIndex = BoardListView.ItemContainerGenerator.IndexFromContainer(itemMoveOn);
        }

        private void BoardListview_MouseEnter(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("mouse enter");
            isMouseEnter = true;
        }

        private void BoardListView_MouseLeave(object sender, MouseEventArgs e)
        {
            //MessageBox.Show("mouse left");
            isMouseEnter = false;
        }



        bool isDirectionDragging = false;
        private void DirectionGrid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            isDirectionDragging = true;
            //MessageBox.Show("flag");
        }

        private void DirectionGrid_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDirectionDragging)
            {
                var position = e.GetPosition(GameGrid);
                //this.Title = position.X.ToString() + " " + position.Y.ToString();

                double totalWidth = GameGrid.ActualWidth;
                double totalHeight = GameGrid.ActualHeight;

                Thickness margin = DirectionGrid.Margin;
                margin.Left = position.X;
                margin.Top = position.Y;
                margin.Right = totalWidth - (position.X + DirectionGrid.ActualWidth);
                margin.Bottom = totalHeight - (position.Y + DirectionGrid.ActualHeight);
                DirectionGrid.Margin = margin;

            }
        }

        private void DirectionGrid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (isDirectionDragging)
            {

                isDirectionDragging = false;
            }
        }

        private void DirectionButton_Click(object sender, RoutedEventArgs e)
        {
            if (isBoardEnabled == false)
            {
                MessageBox.Show("Please click shuffle then click play!");
                return;
            }


            Button button = sender as Button;
            string content = button.Tag as string;

            int lastIndex = -1, newIndex = -1;
            int result = -1;
            if (content.ToLower() == "up".ToLower())
            {
                result = game.ShowWhereCanMoveTo(Game.UpDirection, out lastIndex, out newIndex);
            }
            else if (content.ToLower() == "right".ToLower())
            {
                result = game.ShowWhereCanMoveTo(Game.RightDirection, out lastIndex, out newIndex);
            }
            else if (content.ToLower() == "down".ToLower())
            {
                result = game.ShowWhereCanMoveTo(Game.DownDirection, out lastIndex, out newIndex);
            }
            else if (content.ToLower() == "left".ToLower())
            {
                result = game.ShowWhereCanMoveTo(Game.LeftDirection, out lastIndex, out newIndex);
            }

            if (result != -1)
            {
                // go at index if possible
                this.lastIndex = lastIndex;
                this.newIndex = newIndex;
                //MessageBox.Show($"{lastIndex} {newIndex}");

                // update game process
                gameSemaphore.Release();

            }
        }

        #endregion


        #region game and related threads
        // theads
        Game game;

        Thread gameThread;

        Thread uiUpdateThread;

        private Semaphore gameSemaphore;

        private Semaphore uiUpdateSemaphore;

        private bool isGameStopped = true;


        private void GameProcess()
        {
            try
            {
                game.Start();

                while (game.CheckGameOver() == false)
                {

                    // wait user click
                    gameSemaphore.WaitOne();

                    if (isGameStopped)
                    {
                        break;
                    }

                    // change board
                    game.GoAt(lastIndex, newIndex);

                    // notify update UI thread
                    uiUpdateSemaphore.Release();
                }



                // stop related threads
                isGameStopped = true;
                uiUpdateSemaphore.Release();

            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void UIUpdateProcess()
        {
            try
            {
                while (true)
                {
                    // wait game process
                    uiUpdateSemaphore.WaitOne();

                    

                    // update ui
                    Board board = game.GetCurrentBoard();

                    for (int i = 0; i < listValue.Count; i++)
                    {
                        listValue[i].Value = board.GetAt(i);

                    }
                    AllowUIToUpdate();

                    if (isGameStopped)
                    {
                        break;
                    }
                }


                // show result
                ShowGameResult();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void ShowGameResult()
        {
            isBoardEnabled = false;
            StopTimer();

            if (isShowResult)
            {
                string result = game.GetResult();
                MessageBox.Show(result);
            }
        }

        #endregion


        // window closed event
        private void Window_Closing(object sender, CancelEventArgs e)
        {
            if (!isGameStopped)
            {
                isShowResult = false;
                TerminateGameAndRelatedThreads();
                isGameStopped = true;
            }

            
        }


        #region menu event
        // save & load
        private void SaveGame(string filename)
        {
            // get image name
            string imageName = ImageListView.SelectedItem as string;

            // get second elapsed
            string seconds = countSecond.ToString();

            // get game state
            string gameState = "";
            if (game != null)
            {
                gameState = game.GetSavedState();
            }


            // save
            File.WriteAllText(filename, imageName + "\n" +  countSecond + "\n" + gameState);
        }

        private void LoadGame(string filename)
        {
            // load
            string[] lines = File.ReadAllLines(filename);

            

            // check if image is already in listview
            bool isExist = false;
            foreach (var item in ImageListView.Items)
            {
                string imageName = item as string;
                if (imageName == lines[0])
                {
                    isExist = true;

                    // set selected item
                    ImageListView.SelectedItem = item;

                    // update board
                    NewBoardListView();

                    break;
                }
            }

            if (!isExist)
            {
                // show new image in listview
                UpdateImageListView(lines[0]);


                // set focus to new image
                NewBoardListView();
            }

            // set game state
            game.SetSavedState(lines[2]);

            // set time elapsed
            this.countSecond = Int32.Parse(lines[1]);


            UpdateBoardListView();
            UpdateTimerUI();

        }

        private void LoadMenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new OpenFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;

                bool isSuccess = true;
                try
                {
                    LoadGame(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    isSuccess = false;

                }

                if (isSuccess)
                {
                    MessageBox.Show("Load successfully!");
                }
                else
                {
                    MessageBox.Show("Load failed!");
                }

            }
        }

        private void SaveMenu_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new SaveFileDialog();
            if (dialog.ShowDialog() == true)
            {
                string filename = dialog.FileName;

                bool isSuccess = true;
                try
                {
                    SaveGame(filename);
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                    isSuccess = false;

                }

                if (isSuccess)
                {
                    MessageBox.Show("Save successfully!");
                }
                else
                {
                    MessageBox.Show("Save failed!");
                }
            }
        }

        private void HelpMenu_Click(object sender, RoutedEventArgs e)
        {
            string info = "--- A project from Windows Programming Course ---";
            string performer = "Performer: 1753102 - Bui Quang Thang, HCMUS";
            string contact = "Email: 1753102@student.hcmus.edu.vn";
            string dateComplete = "Lastest version: version 2.0 Arbitrary - 1/8/2019";

            StringBuilder builder = new StringBuilder();
            builder.AppendLine(info);
            builder.AppendLine();

            builder.AppendLine(dateComplete);
            builder.AppendLine(performer);
            builder.AppendLine(contact);
            

            MessageBox.Show(builder.ToString());
        }

        private void OptionsMenu_Click(object sender, RoutedEventArgs e)
        {
            OptionsWindow child = new OptionsWindow();
            if (child.ShowDialog() == true)
            {
                if (child.RowsOption >= 2 && child.RowsOption <= 20)
                {
                    int newRows = child.RowsOption;
                    Game.Rows = newRows;

                    MessageBox.Show($" Set board to {newRows} x {newRows} successfully!\nPlease select another image to see effect!");
                }

            }
        }




        #endregion

        
    }
}
