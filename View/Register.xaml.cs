using System;
using System.Collections.Generic;
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
using System.Text.RegularExpressions;
using System.IO;

namespace FinalDbProject
{
    /// <summary>
    /// Interaction logic for Register.xaml
    /// </summary>
    public partial class Register : Page
    {
        private RegisterController register_c; // The register controller
        private static Register instance = null; // The self object


        /// <summary>
        /// The constructor.
        /// </summary>
        private Register()
        {
            InitializeComponent();

            // Add the background image
            ImageBrush myBrush = new ImageBrush();
            Image image = new Image();
            image.Source = new BitmapImage(
                new Uri(Directory.GetCurrentDirectory() + "\\Resources\\BackgroundImages\\RegisterBG.jpg"));
            myBrush.ImageSource = image.Source;
            MainGrid.Background = myBrush;

            this.register_c = new RegisterController();
        }


        /// <summary>
        /// Return the self object.
        /// </summary>
        /// <returns></returns>
        public static Register Instance()
        {
            if (instance == null)
            {
                instance = new Register();
            }
            return instance;
        }


        /// <summary>
        /// If there is an error occure so move to the exception page.
        /// </summary>
        /// <param name="e"></param>
        private void MoveToException(Exception e)
        {
            this.NavigationService.Navigate(new ExceptionPage(e));
        }


        /// <summary>
        /// The method checks if the username is exist.
        /// </summary>
        /// <returns>true - If the username is exist. false - Otherwise.</returns>
        private bool IsExistUsername()
        {
            this.register_c.SetUserName(TextBoxUsernameInput.Text);
            bool result = false;
            try
            {
                result = this.register_c.IsUsernameExist(); // check if the given username exist
                if (result)
                {
                    LabelUsernameErrorMessage.Content = "The username is exist";
                    return true;
                }

                return false;
            } catch(Exception e)
            {
                this.MoveToException(e); // exception occured
                return false;
            }
        }


        /// <summary>
        /// The method checks the validation of the given user inputs
        /// (username, name and password).
        /// </summary>
        /// <returns> true - If the username, name and password are valid.
        ///           false - Otherwise.
        /// </returns>
        private bool CheckContentValidation()
        {
            LabelUsernameErrorMessage.Content = ""; // Reset
            LabelPasswordErrorMessage.Content = ""; // Reset
            LabelNameErrorMessage.Content = ""; // Reset

            bool answer = true; // The answer

            // If the username is empty
            if (TextBoxUsernameInput.Text == "")
            {
                LabelUsernameErrorMessage.Content = "Missing Username";
                answer = false;
            }
            // Check validation of username (only English letters or numbers)
            else if (!Regex.IsMatch(TextBoxUsernameInput.Text, "^[a-zA-Z0-9]*$"))
            {
                LabelUsernameErrorMessage.Content = "Invalid username";
                answer = false;
            }

            // If the name is empty
            if (TextBoxNameInput.Text == "")
            {
                LabelNameErrorMessage.Content = "Missing Name";
                answer = false;
            }
            // Check validation of name (only English letters [and spaces if indeed])
            else if (!Regex.IsMatch(TextBoxNameInput.Text, "^[A-Za-z]+( [A-Za-z]+)*$"))
            {
                LabelNameErrorMessage.Content = "Invalid name";
                answer = false;
            }

            // If the password is empty
            if (TextBoxPasswordInput.Password == "")
            {
                LabelPasswordErrorMessage.Content = "Missing Password";
                answer = false;
            }
            // Check validation of password (only English letters or numbers)
            else if (!Regex.IsMatch(TextBoxPasswordInput.Password, "^[a-zA-Z0-9]*$"))
            {
                LabelPasswordErrorMessage.Content = "Invalid password";
                answer = false;
            }

            if (answer == false)
            {
                return false;
            }

            return !IsExistUsername();
        }


        /// <summary>
        /// The method treats the event when the register button is clicked.
        /// If the given input is valid and correct, we go to the Game Menu
        /// (and we are loggined).
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonRegister_Click(object sender, RoutedEventArgs e)
        {

            // Check if the data is valid (and wirte the error messages if they are needed)
            if (this.CheckContentValidation())
            {
                this.register_c.SetName(TextBoxNameInput.Text);
                this.register_c.SetUserName(TextBoxUsernameInput.Text);
                Console.WriteLine(TextBoxPasswordInput.Password);
                this.register_c.SetPassword(TextBoxPasswordInput.Password);

                try
                {
                    this.register_c.Register();

                    // REDIRECT TO THE GAME_MENU (AND THE USER AUTOMATICALLY IS LOGGINED)
                    GameMenu gm = GameMenu.Instance();
                    gm.setData(TextBoxUsernameInput.Text, TextBoxNameInput.Text);
                    this.NavigationService.Navigate(gm);
                }
                catch (Exception exception)
                {
                    this.MoveToException(exception);
                }
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