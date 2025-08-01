using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using MovieRental.WpfApp.ViewModels;

namespace MovieRental.WpfApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool _customersLoaded = false;
        private bool _paymentMethodsLoaded = false;

        private readonly HttpClient _httpClient;
        public MainWindow()
        {
            _httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7271") // Or wherever your API is hosted
            };

            InitializeComponent();
        }

        public async Task<List<MovieDto>> GetMoviesAsync()
        {
            return await _httpClient.GetFromJsonAsync<List<MovieDto>>("/Movie");
        }

        
        private async void LoadMoviesButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var movies = await GetMoviesAsync();
                MoviesListBox.ItemsSource = movies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading movies: {ex.Message}");
            }
        }

        private async void AddMovieButton_Click(object sender, RoutedEventArgs e)
        {
            var title = MovieTitleTextBox.Text;

            if (string.IsNullOrWhiteSpace(title))
            {
                MessageBox.Show("Please enter a movie title.");
                return;
            }

            var newMovie = new MovieDto { Title = title };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/Movie", newMovie);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Movie added successfully!");
                MovieTitleTextBox.Clear();

                // Reload the list
                var movies = await GetMoviesAsync();
                MoviesListBox.ItemsSource = movies;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding movie: {ex.Message}");
            }
        }


        private async void AddCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            var name = CustomerNameTextBox.Text.Trim();
            var email = CustomerEmailTextBox.Text.Trim();
            var phone = CustomerPhoneTextBox.Text.Trim();
            var address = CustomerAddressTextBox.Text.Trim();

            if (string.IsNullOrWhiteSpace(name) ||
                string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(phone) ||
                string.IsNullOrWhiteSpace(address))
            {
                MessageBox.Show("Todos os campos do cliente são obrigatórios.", "Validação", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newCustomer = new CustomerDto
            {
                Name = CustomerNameTextBox.Text,
                Email = CustomerEmailTextBox.Text,
                Phone = CustomerPhoneTextBox.Text,
                Address = CustomerAddressTextBox.Text,
                CreatedAt = DateTime.UtcNow
            };

            

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/Customer", newCustomer);
                response.EnsureSuccessStatusCode();

                MessageBox.Show("Customer added successfully!");

                // Clear form
                CustomerNameTextBox.Clear();
                CustomerEmailTextBox.Clear();
                CustomerPhoneTextBox.Clear();
                CustomerAddressTextBox.Clear();

                // Reload list
                var customers = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("/Customer");
                CustomersListBox.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error adding customer: {ex.Message}");
            }
        }

        private async void LoadCustomersButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var customers = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("/Customer");
                CustomersListBox.ItemsSource = customers;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error loading customers: {ex.Message}");
            }
        }

        private async void CreateRentalButton_Click(object sender, RoutedEventArgs e)
        {
            if (CustomerComboBox.SelectedItem is not CustomerDto selectedCustomer ||
                MovieComboBox.SelectedItem is not MovieDto selectedMovie ||
                !int.TryParse(DaysRentedTextBox.Text, out int days))
            {
                MessageBox.Show("Please fill all fields correctly.");
                return;
            }

            var paymentItem = PaymentMethodComboBox.SelectedItem as ComboBoxItem;
            var paymentMethod = paymentItem?.Content?.ToString() ?? "MbWay";

            var rental = new RentalDto
            {
                CustomerId = selectedCustomer.Id,
                MovieId = selectedMovie.Id,
                DaysRented = days,
                PaymentMethod = paymentMethod
            };

            try
            {
                var response = await _httpClient.PostAsJsonAsync("/Rental", rental);
                response.EnsureSuccessStatusCode();
                if (response.IsSuccessStatusCode)
                {
                    MessageBox.Show("Rental created!");

                    CustomerComboBox.SelectedIndex = -1;
                    MovieComboBox.SelectedIndex = -1;
                    DaysRentedTextBox.Text = string.Empty;
                    PaymentMethodComboBox.SelectedIndex = -1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error: " + ex.Message);
            }
        }

        private async void GetRentalsButton_Click(object sender, RoutedEventArgs e)
        {
            string name = SearchCustomerNameTextBox.Text.Trim();
            if (string.IsNullOrEmpty(name)) return;

            try
            {
                var rentals = await _httpClient.GetFromJsonAsync<List<RentalDto>>($"/Rental/customer/{name}");
                RentalsListBox.ItemsSource = rentals
                    .Select(r => $"CustomerId: {r.CustomerId} | MovieId: {r.MovieId} | Days: {r.DaysRented} | Pay: {r.PaymentMethod}")
                    .ToList();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to get rentals: " + ex.Message);
            }
        }


        
        private async void CustomerComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (_customersLoaded) return;

            try
            {
                var customers = await _httpClient.GetFromJsonAsync<List<CustomerDto>>("/Customer");
                CustomerComboBox.ItemsSource = customers;
                _customersLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load customers: " + ex.Message);
            }
        }

        private bool _moviesLoaded = false;
        private async void MovieComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (_moviesLoaded) return;

            try
            {
                var movies = await _httpClient.GetFromJsonAsync<List<MovieDto>>("/Movie");
                MovieComboBox.ItemsSource = movies;
                _moviesLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load movies: " + ex.Message);
            }
        }

        private async void PaymentMethodComboBox_DropDownOpened(object sender, EventArgs e)
        {
            if (_paymentMethodsLoaded) return;

            try
            {
                var paymentMethods = await _httpClient.GetFromJsonAsync<List<string>>("/Payment/methods");
                PaymentMethodComboBox.ItemsSource = paymentMethods;
                _moviesLoaded = true;
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to load movies: " + ex.Message);
            }
        }


    }
}