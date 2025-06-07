using Shop.Models;
using Shop.Services;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace Shop.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged
    {
        private readonly ShopDbService _dbService;
        private ObservableCollection<Product> _products = new ObservableCollection<Product>();
        private ObservableCollection<Product> _filteredProducts = new ObservableCollection<Product>();
        private string _searchQuery;

        public ObservableCollection<Product> Products
        {
            get => _products;
            set
            {
                _products = value;
                OnPropertyChanged();
                UpdateFilteredProducts();
            }
        }

        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set
            {
                _filteredProducts = value;
                OnPropertyChanged();
            }
        }

        public string SearchQuery
        {
            get => _searchQuery;
            set
            {
                _searchQuery = value;
                OnPropertyChanged();
                UpdateFilteredProducts();
            }
        }

        public CatalogViewModel(ShopDbService dbService)
        {
            _dbService = dbService;
            _ = InitializeAsync();
        }

        public async Task InitializeAsync()
        {
            await LoadProducts();
        }

        private async Task LoadProducts()
        {
            try
            {
                var products = await _dbService.GetAllProductsAsync();
                Debug.WriteLine($"Получено товаров из БД: {products?.Count ?? 0}");
                if (products != null)
                {
                    foreach (var p in products)
                    {
                        Debug.WriteLine($"Товар: {p.Name}, Цена: {p.Price}");
                    }
                }
                Products = new ObservableCollection<Product>(products);
                Debug.WriteLine($"Товаров в Products: {Products.Count}");
                Debug.WriteLine($"Товаров в FilteredProducts: {FilteredProducts.Count}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Ошибка загрузки товаров: {ex.Message}");
            }
        }

        private void UpdateFilteredProducts()
        {
            if (Products == null) return;

            var filtered = string.IsNullOrWhiteSpace(SearchQuery)
                ? Products
                : Products.Where(p =>
                    p.Name.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ||
                    (p.Description?.Contains(SearchQuery, StringComparison.OrdinalIgnoreCase) ?? false));

            FilteredProducts = new ObservableCollection<Product>(filtered);
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}