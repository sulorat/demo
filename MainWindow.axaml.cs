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

    private int _sortResult = 0;

    [SuppressMessage("ReSharper.DPA", "DPA0000: DPA issues")]
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
        
        SortComboBox.ItemsSource = _sortValue;
        DisplayService();
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
        ServiceListBox.ItemsSource = DisplayList;
        StatisticTextBlock.Text = $"{DisplayList.Count} из {SourceList.Count}";
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