using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMall.Services.DTOs;
using MyMall.Services.Interfaces;
using MyMall.ViewModels.Base;

namespace MyMall.ViewModels.Admin;

public partial class CategoriesViewModel : ViewModelBase
{
    private readonly ICategoryService _categoryService;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    private CategoryDto? _selectedCategory;
    public CategoryDto? SelectedCategory
    {
        get => _selectedCategory;
        set
        {
            if (SetProperty(ref _selectedCategory, value))
            {
                OnPropertyChanged(nameof(IsCategorySelected));
            }
        }
    }

    public bool IsCategorySelected => SelectedCategory != null;

    private string _newCategoryName = string.Empty;
    public string NewCategoryName
    {
        get => _newCategoryName;
        set => SetProperty(ref _newCategoryName, value);
    }

    private string? _newCategoryDescription;
    public string? NewCategoryDescription
    {
        get => _newCategoryDescription;
        set => SetProperty(ref _newCategoryDescription, value);
    }

    private string? _errorMessage;
    public string? ErrorMessage
    {
        get => _errorMessage;
        set => SetProperty(ref _errorMessage, value);
    }

    private bool _isLoading;
    public bool IsLoading
    {
        get => _isLoading;
        set => SetProperty(ref _isLoading, value);
    }

    public CategoriesViewModel(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    [RelayCommand]
    public async Task LoadAsync()
    {
        IsLoading = true;
        ErrorMessage = null;

        try
        {
            var categories = await _categoryService.GetAllAsync();

            Categories.Clear();
            foreach (var c in categories) Categories.Add(c);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка загрузки: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private async Task AddAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
            ErrorMessage = "Введите название категории";
            return;
        }

        try
        {
            var dto = new CategoryDto
            {
                Name = NewCategoryName,
                Description = NewCategoryDescription
            };

            await _categoryService.CreateAsync(dto);

            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task UpdateAsync()
    {
        if (SelectedCategory == null) return;

        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(NewCategoryName))
        {
            ErrorMessage = "Введите название категории";
            return;
        }

        try
        {
            var dto = new CategoryDto
            {
                Id = SelectedCategory.Id,
                Name = NewCategoryName,
                Description = NewCategoryDescription
            };

            await _categoryService.UpdateAsync(SelectedCategory.Id, dto);

            NewCategoryName = string.Empty;
            NewCategoryDescription = string.Empty;
            SelectedCategory = null;

            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (SelectedCategory == null) return;

        var result = System.Windows.MessageBox.Show(
            $"Удалить категорию '{SelectedCategory.Name}'?",
            "Подтверждение",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Question);

        if (result != System.Windows.MessageBoxResult.Yes) return;

        ErrorMessage = null;

        try
        {
            await _categoryService.DeleteAsync(SelectedCategory.Id);
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Ошибка удаления: {ex.Message}";
        }
    }

    [RelayCommand]
    private void SelectCategory(CategoryDto? category)
    {
        SelectedCategory = category;

        if (category != null)
        {
            NewCategoryName = category.Name;
            NewCategoryDescription = category.Description;
        }
    }

    [RelayCommand]
    private void ClearForm()
    {
        NewCategoryName = string.Empty;
        NewCategoryDescription = string.Empty;
        SelectedCategory = null;
        ErrorMessage = null;
    }
}