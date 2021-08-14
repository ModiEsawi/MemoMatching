using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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

namespace FinalDbProject
{
    /// <summary>
    /// Interaction logic for Login.xaml
    /// </summary>
    public partial class Login : Page
    {
        private LoginController login_c; // The controller of the login
        private static Login instance = null; // The self object


        /// <summary>
        /// The constructor.
        /// </summary>
        private Login()
        {
            InitializeComponent();

            // Add the background image
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\LoginBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;

            this.login_c = new LoginController();
        }


        /// <summary>
        /// Return the self object.
        /// </summary>
        /// <returns></returns>
        public static Login Instance()
        {
            if (instance == null)
            {
                instance = new Login();
            }
            return instance;
        }


        /// <summary>
        /// The method checks if the username and its password are correct.
        /// </summary>
        /// <returns> true - If the user and password are correct. false - Otherwise./returns>
        private string[] IsExistUser()
        {
            this.login_c.SetUserName(this.TextBoxUsernameInput.Text); // update the username
            string[] queryAnswer = null;
            try
            {
                queryAnswer = this.login_c.GetUserNameDetails();
                // If the user is not exist
                if (queryAnswer == null)
                {
                    LabelUsernameErrorMessage.Content = "The user is not exist";
                    return null;
                }


                // If the password is not correct
                if (queryAnswer[2] != TextBoxPasswordInput.Password)
                {
                    LabelPasswordErrorMessage.Content = "The password is not correct";
                    return null;
                }

                return queryAnswer;
            }
            catch (Exception e)
            {
                this.NavigationService.Navigate(new ExceptionPage(e));
                return null;
            }
        }


        /// <summary>
        /// The method checks the validation of the given user inputs
        /// (username and password).
        /// </summary>
        /// <returns> true - If both, username and password are valid.
        ///           false - Otherwise.
        /// </returns>
        private string[] CheckContentValidation()
        {
 
            LabelUsernameErrorMessage.Content = ""; // Reset
            LabelPasswordErrorMessage.Content = ""; // Reset

            bool answer = true; // The answer

            // If the username is empty
            if (TextBoxUsernameInput.Text == "")
            {
                LabelUsernameErrorMessage.Content = "Missing Username";
                answer = false;
            }

            // If the password is empty
            if (TextBoxPasswordInput.Password == "")
            {
                LabelPasswordErrorMessage.Content = "Missing Password";
                answer = false;
            }

            if (answer == false)
            {
                return null;
            }

            return IsExistUser();
        }


        /// <summary>
        /// The method treats the event when the login button is clicked.
        /// If the given input is valid and correct, we go to the Game Menu.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonLogin_Click(object sender, RoutedEventArgs e)
        {
            string[] ans = this.CheckContentValidation(); // The data about the user

            // Check if the data is valid (and wirte the error messages if they are needed)
            if (ans != null)
            {
                // REDIRECT TO THE GAME_MENU (WHEN THE USER IS LOGIN)
                GameMenu gm = GameMenu.Instance();
                gm.setData(ans[0], ans[1]);
                this.NavigationService.Navigate(gm);
            }

        }


        /// <summary>
        /// Go back to the MainWindow.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonBack_Click(object sender, RoutedEventArgs e)
        {
            this.NavigationService.GoBack();
        }
    }
}
