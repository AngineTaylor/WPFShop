using Shop.Data;
using Shop.Models;
using Shop.ViewModels;
using Shop.Commands;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using System.Windows.Input;
using System.Linq;

namespace Shop.ViewModels
{
    public class CatalogViewModel : INotifyPropertyChanged
    {
        private readonly AppDbContext _dbContext;
        private readonly CartViewModel _cartViewModel;

        public CatalogViewModel(AppDbContext dbContext, CartViewModel cartViewModel)
        {
            _dbContext = dbContext;
            _cartViewModel = cartViewModel;

            AddToCartCommand = new RelayCommand(AddToCart);

            // Инициализация коллекций
            Products = new ObservableCollection<Product>();
            FilteredProducts = new ObservableCollection<Product>();
            Categories = new ObservableCollection<Category>();

            // Загрузка данных из базы
            LoadData();
        }

        // Все товары из базы
        public ObservableCollection<Product> Products { get; set; }

        // Отфильтрованные товары для отображения
        private ObservableCollection<Product> _filteredProducts = new();
        public ObservableCollection<Product> FilteredProducts
        {
            get => _filteredProducts;
            set
            {
                if (_filteredProducts != value)
                {
                    _filteredProducts = value;
                    OnPropertyChanged();
                }
            }
        }

        // Категории из базы
        public ObservableCollection<Category> Categories { get; set; }

        // Выбранная категория для фильтрации
        private Category? _selectedCategory;
        public Category? SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (_selectedCategory != value)
                {
                    _selectedCategory = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        // Текст поиска
        private string _searchText = string.Empty;
        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    ApplyFilter();
                }
            }
        }

        public ICommand AddToCartCommand { get; }

        private void AddToCart(object? parameter)
        {
            if (parameter is Product product)
            {
                _cartViewModel.AddProduct(product);
            }
        }

        private async void LoadData()
        {
            var categories = await _dbContext.Categories.ToListAsync();
            Categories.Clear();
            Categories.Add(new Category { Id = 0, Name = "Все категории" }); // Для выбора без фильтра
            foreach (var c in categories)
                Categories.Add(c);

            var products = await _dbContext.Products.ToListAsync();
            Products.Clear();
            foreach (var p in products)
                Products.Add(p);

            SelectedCategory = Categories.FirstOrDefault(); // По умолчанию "Все категории"
            ApplyFilter();
        }

        private void ApplyFilter()
        {
            var filtered = Products.AsEnumerable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                filtered = filtered.Where(p => p.Name != null && p.Name.Contains(SearchText, System.StringComparison.OrdinalIgnoreCase));
            }

            if (SelectedCategory != null && SelectedCategory.Id != 0) // 0 - "Все категории"
            {
                filtered = filtered.Where(p => p.CategoryId == SelectedCategory.Id);
            }

            FilteredProducts = new ObservableCollection<Product>(filtered);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string? name = null) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    }
}
