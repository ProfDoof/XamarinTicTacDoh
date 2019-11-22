using Android.App;
using Android.OS;
using Android.Support.V7.App;
using Android.Runtime;
using Android.Widget;
using Android.Views;
using Android.Graphics.Drawables;
using Java.Lang;
using System.Collections.Generic;

namespace TicTacDoh
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, View.IOnClickListener
    {
        private ImageButton[,] buttons = new ImageButton[3,3];
        
        private bool player1Turn = true;

        private int roundCount = 0;

        private int player1Points = 0;
        private int player2Points = 0;

        private TextView textViewPlayer1;
        private TextView textViewPlayer2;

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            // Set our view from the "main" layout resource
            SetContentView(Resource.Layout.activity_main);

            textViewPlayer1 = FindViewById<TextView>(Resource.Id.player_1_text_view);
            textViewPlayer2 = FindViewById<TextView>(Resource.Id.player_2_text_view);

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    string buttonId = "button_" + row.ToString() + column.ToString();
                    int buttonResId = Resources.GetIdentifier(buttonId, "id", PackageName);

                    buttons[row, column] = FindViewById<ImageButton>(buttonResId);
                    buttons[row, column].SetOnClickListener(this);
                    buttons[row, column].Tag = Resource.Drawable.blank_black_button;
                }
            }

            setTitleBarColorForPlayerTurn();

            Button buttonReset = FindViewById<Button>(Resource.Id.button_reset);
            buttonReset.SetOnClickListener(new ButtonResetOnClickListener(this));
        }

        class ButtonResetOnClickListener: Java.Lang.Object, View.IOnClickListener
        {
            MainActivity m_mainActivity;
            public ButtonResetOnClickListener(MainActivity mainActivity)
            {
                m_mainActivity = mainActivity;
            }
            public void OnClick(View v)
            {
                m_mainActivity.resetGameState();
            }
        }

        public void OnClick(View v)
        {
            ImageButton view = v as ImageButton;
            if (! ( (int) view.Tag == Resource.Drawable.blank_black_button))
            {
                return;
            }

            if (player1Turn)
            {
                view.SetImageResource(Resource.Drawable.mandelbrot);
                view.Tag = Resource.Drawable.mandelbrot;
            }
            else
            {
                view.SetImageResource(Resource.Drawable.julia);
                view.Tag = Resource.Drawable.julia;
            }

            roundCount++;
            if (checkIfPlayerWon())
            {
                if (player1Turn)
                {
                    player1Won();
                }
                else
                {
                    player2Won();
                }
            }
            else if (roundCount == 9)
            {
                catGame();
            }
            else
            {
                player1Turn = !player1Turn;
                setTitleBarColorForPlayerTurn();
            }
        }

        private bool checkIfPlayerWon()
        {
            int[,] currentBoardState = new int[3, 3];

            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    currentBoardState[row, column] = (int)buttons[row, column].Tag;
                }
            }

            for (int row = 0; row < 3; row++)
            {
                if (currentBoardState[row, 0] == currentBoardState[row, 1]
                 && currentBoardState[row, 0] == currentBoardState[row, 2]
                 && !(currentBoardState[row, 0] == Resource.Drawable.blank_black_button))
                {
                    return true;
                }
            }

            for (int column = 0; column < 3; column++)
            {
                if (currentBoardState[0, column] == currentBoardState[1, column]
                 && currentBoardState[0, column] == currentBoardState[2, column]
                 && !(currentBoardState[0, column] == Resource.Drawable.blank_black_button))
                {
                    return true;
                }
            }

            if (currentBoardState[0, 0] == currentBoardState[1, 1]  
             && currentBoardState[0, 0] == currentBoardState[2, 2]
             && !(currentBoardState[0, 0] == Resource.Drawable.blank_black_button))
            {
                return true;
            }

            if (currentBoardState[0, 2] == currentBoardState[1, 1]
             && currentBoardState[0, 2] == currentBoardState[2, 0]
             && !(currentBoardState[0, 2] == Resource.Drawable.blank_black_button))
            {
                return true;
            }

            return false;
        }

        private void player1Won()
        {
            player1Points++;
            Toast.MakeText(this, "Player 1 Wins!", ToastLength.Long).Show();
            updatePlayerPoints();
            resetBoardState();
        }

        private void player2Won()
        {
            player2Points++;
            Toast.MakeText(this, "Player 2 Wins!", ToastLength.Long).Show();
            updatePlayerPoints();
            resetBoardState();
        }

        private void catGame()
        {
            Toast.MakeText(this, "Cat Game!", ToastLength.Long).Show();
            resetBoardState();
        }

        private void updatePlayerPoints()
        {
            string text1 = "Player 1: " + player1Points.ToString();
            string text2 = "Player 2: " + player2Points.ToString();
            textViewPlayer1.SetText(text1.ToCharArray(), 0, text1.Length);
            textViewPlayer2.SetText(text2.ToCharArray(), 0, text2.Length);
        }

        private void resetBoardState()
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    buttons[row, column].SetImageResource(Resource.Drawable.blank_black_button);
                    buttons[row, column].Tag = Resource.Drawable.blank_black_button;
                }
            }

            player1Turn = true;
            roundCount = 0;

            setTitleBarColorForPlayerTurn();
        }

        private void resetGameState()
        {
            resetBoardState();
            player1Points = 0;
            player2Points = 0;
            updatePlayerPoints();
        }

        override protected void OnSaveInstanceState(Bundle outState)
        {
            base.OnSaveInstanceState(outState);

            outState.PutInt("roundCount", roundCount);
            outState.PutInt("player1Points", player1Points);
            outState.PutInt("player2Points", player2Points);
            outState.PutBoolean("player1Turn", player1Turn);
            outState.PutIntegerArrayList("imageButtonsState", saveImageButtonsState());
        }

        override protected void OnRestoreInstanceState(Bundle savedInstanceState)
        {
            base.OnRestoreInstanceState(savedInstanceState);

            roundCount = savedInstanceState.GetInt("roundCount");
            player1Points = savedInstanceState.GetInt("player1Points");
            player2Points = savedInstanceState.GetInt("player2Points");
            player1Turn = savedInstanceState.GetBoolean("player1Turn");
            List<Integer> savedImageButtonState = savedInstanceState.GetIntegerArrayList("imageButtonsState") as List<Integer>;
            restoreImageButtonsState(savedImageButtonState);
            setTitleBarColorForPlayerTurn();
        }

        private List<Integer> saveImageButtonsState()
        {
            List<Integer> imageButtonsResourceIds = new List<Integer>(9);
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    imageButtonsResourceIds.Add((Integer) buttons[row, column].Tag);
                }
            }

            return imageButtonsResourceIds;
        }

        private void restoreImageButtonsState(List<Integer> imageButtonsResourceIds)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int column = 0; column < 3; column++)
                {
                    buttons[row, column].SetImageResource((int) imageButtonsResourceIds[(row * 3) + column]);
                    buttons[row, column].Tag = (int)imageButtonsResourceIds[(row * 3) + column];
                }
            }
        }

        private void setTitleBarColorForPlayerTurn()
        {
            if (player1Turn)
            {
                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.mandelbrotRed)));
                SupportActionBar.SetTitle(Resource.String.player_1_turn);
            }
            else
            {
                SupportActionBar.SetBackgroundDrawable(new ColorDrawable(Resources.GetColor(Resource.Color.juliaBlue)));
                SupportActionBar.SetTitle(Resource.String.player_2_turn);
            }
        }

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
    }
}