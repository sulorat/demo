using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using System.Linq;
using demo.EntityModels;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SkiaSharp;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Media;
using Npgsql.TypeMapping;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using Avalonia.Interactivity;

namespace demo;


public class ServicePresenter() : Service
{
    public int ServiceId
    {
        get => this.Id;
    }

    public string ServiceName
    {
        get => this.Title;
    }

    public string ServiceCost
    {
        get =>
            (this.Discount != 0
                ? $"{this.Cost - this.Cost * Convert.ToDecimal(this.Discount):0.00} рублей за {this.Durationinseconds} минут"
                : $" {this.Cost:0} рублей за {this.Durationinseconds} минут"
            );
    }

    public Bitmap ServiceImage { get => GetBitmap(this.Mainimagepath); }

    public string ServiceDiscount
    {
        get =>
            (this.Discount != 0
                ? string.Format("скидка {0}%", this.Discount*100)
                : String.Empty
            );
    }

    public decimal? ServiceRealCost
    {
        get => this.Discount == 0 
            ? this.Cost 
            : null;
    }


    private Bitmap GetBitmap(string fileName) {
        try {
            return new Bitmap($"Assets\\{fileName}");
        }
        catch (Exception ex) {
            return new Bitmap("Assets\\service_logo.ico");

        }
    }
}

public partial class MainWindow : Window
{
    private string _searchWord = String.Empty;

   
    private List<ServicePresenter> DisplayList { get; set; } = new List<ServicePresenter>();
    private List<ServicePresenter> SourceList { get; set; }

    
    private ObservableCollection<string> _sortValue = new ObservableCollection<string>()
        { "все", "по возрастанию", "по убыванию" };

    private ObservableCollection<string> _filterValue = new ObservableCollection<string>()
    {
        "Все","0-5%","5-15%","15-30%","30-70%","70-100%"
    };
    
    private int filterResult = 0;

    private int _sortResult = 0;

    public MainWindow()
    {
        InitializeComponent();
        using (var dbContext = new ShaluhinContext())
        {
            SourceList = dbContext.Services.Select(service =>
                new ServicePresenter
                {
                    Title = service.Title,
                    Discount = service.Discount,
                    Durationinseconds = service.Durationinseconds,
                    Id = service.Id,
                    Cost = service.Cost,
                    Mainimagepath = service.Mainimagepath,
                }
            ).ToList();
        }
        FilterSalesComboBox.ItemsSource = _filterValue;
        SortComboBox.ItemsSource = _sortValue;
        DisplayService();
    }

    public MainWindow(bool admidMode)
    {

    }
    private void DisplayService()
    {
        DisplayList = new List<ServicePresenter>(SourceList);
        if (!string.IsNullOrEmpty(_searchWord))
        {
            DisplayList = DisplayList.Where(x => x.ServiceName.ToLower().Contains(_searchWord)).ToList();
        }
        switch (_sortResult)    
        {
            case 0:
                DisplayList = DisplayList;
                break;
            case 1:
                DisplayList = DisplayList.OrderBy(x => x.Cost).ToList();
                break;
            case 2 :
                DisplayList = DisplayList.OrderByDescending(x => x.Cost).ToList();
                break;
        }
        switch (filterResult)
        {
            case 0:
                DisplayList = DisplayList;
                    break;
            case 1:
                DisplayList = (DisplayList.Where(x => (x.Discount*100 >= 0 && x.Discount*100 < 5)).ToList());
                break;
            case 2:
                DisplayList = (DisplayList.Where(x => (x.Discount*100 >= 5 && x.Discount*100 < 15)).ToList());
                break;
            case 3:
                DisplayList = (DisplayList.Where(x => (x.Discount*100 >= 15 && x.Discount*100 < 30)).ToList());
                break;
            case 4:
                DisplayList = (DisplayList.Where(x => (x.Discount*100 >= 30 && x.Discount*100 < 70)).ToList());
                break;
            case 5:
                DisplayList = (DisplayList.Where(x => (x.Discount*100 >= 70 && x.Discount*100 < 100)).ToList());
                break;
            
        }
        ServiceListBox.ItemsSource = DisplayList;
        StatisticTextBlock.Text = $"{DisplayList.Count} из {SourceList.Count}";
    }

    private void FilteringComboBox_SelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        filterResult = (sender as ComboBox).SelectedIndex;
        DisplayService();
        
    }
    private void ToAdmin_Click(object sender, RoutedEventArgs e)
    {
        var adminPage = new ToAdmin();
        adminPage.Show();
        this.Close(); 
    }

    private void SortComboBox_OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        
        int filtrValue =  (sender as ComboBox).SelectedIndex;
        switch (filtrValue)
        {
            case 0:
                _sortResult = 0;
                break;
            case 1:
                _sortResult = 1;
                break;
            case 2:
                _sortResult = 2;
                break;
        }
        DisplayService();
    }

    private void SearchingTextbox(object? sender, TextChangingEventArgs e)
    {
        _searchWord = SerchingTextBox.Text.ToLower();
        DisplayService();
    }
}