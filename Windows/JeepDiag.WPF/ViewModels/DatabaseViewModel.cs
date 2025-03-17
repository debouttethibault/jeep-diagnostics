using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using DRBDBReader.Db;
using DRBDBReader.Db.Records;
using Microsoft.Win32;

namespace JeepDiag.WPF.ViewModels;

public partial class DatabaseViewModel : ObservableObject, INavigatableViewModel
{
    public string Title
    {
        get
        {
            if (ModuleListVisibility == Visibility.Visible)
                return "Database Modules";
            if (CommandListVisibility == Visibility.Visible)
                return $"Module: {_selectedModule!.Value.Value}";
            return string.Empty;
        }
    }
    
    public Visibility ModuleListVisibility => Modules != null && !SelectedModuleId.HasValue 
                                                ? Visibility.Visible : Visibility.Hidden;
    public Visibility CommandListVisibility => SelectedModuleId.HasValue && ModuleCommands != null
                                                ? Visibility.Visible : Visibility.Hidden;

    public bool IsModuleSelected => _selectedModuleId.HasValue;

    private KeyValuePair<ushort, string>? _selectedModule;
    
    [ObservableProperty] private bool _isDatabaseLoaded;
    [ObservableProperty] private bool _isFileSelected;
    private int? _selectedModuleId;
    public int? SelectedModuleId
    {
        get => _selectedModuleId;
        set
        {
            
            if (!value.HasValue || Modules == null 
                                || value.Value < 0  || value.Value > Modules.Count)
                SetProperty(ref _selectedModuleId, null);
            else
                SetProperty(ref _selectedModuleId, value);

            if (_selectedModuleId.HasValue 
                    && Modules != null)
                _selectedModule = Modules[_selectedModuleId.Value];
            
            OnPropertyChanged(nameof(IsModuleSelected));
            OnPropertyChanged(nameof(ModuleListVisibility));
            OnPropertyChanged(nameof(CommandListVisibility));
            OnPropertyChanged(nameof(Title));
            
            LoadModTxList();
        }
    }
    [ObservableProperty] 
    [NotifyPropertyChangedFor(nameof(CommandListVisibility))] 
    [NotifyPropertyChangedFor(nameof(Title))] 
    private List<KeyValuePair<uint, string>>? _moduleCommands;
    [NotifyPropertyChangedFor(nameof(ModuleListVisibility))]
    [NotifyPropertyChangedFor(nameof(Title))] 
    [ObservableProperty] private List<KeyValuePair<ushort, string>>? _modules;
    
    private FileInfo? _file;
    private Database? _database;
    
    [RelayCommand]
    private void LoadDatabase()
    {
        if (_file == null)
        {
            IsFileSelected = false;
            return;
        }

        try
        {
            _database = new Database(_file);
            IsDatabaseLoaded = true;
            LoadModules();
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message,"Failed loading database", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void DeselectModule()
    {
        SelectedModuleId = null;
    }

    [RelayCommand]
    private void LoadDatabaseFile()
    {
        var dialog = new OpenFileDialog();
        if (dialog.ShowDialog() == true)
        {
            _file = new FileInfo(dialog.FileName);
            IsFileSelected = true;
            LoadDatabase();
            return;
        }
        IsFileSelected = false;
    }

    private void LoadModTxList()
    {
        if (_database == null || !_selectedModule.HasValue)
            return;

        if (ModuleCommands == null)
            ModuleCommands = new List<KeyValuePair<uint, string>>(100);
        else
            ModuleCommands.Clear();

        var table = _database.tables[Database.TABLE_MODULE];
        if (table.getRecord(_selectedModule.Value.Key) is ModuleRecord record)
        {
            foreach (var txRecord in record.dataelements)
            {
                var tx = _database.getTX(txRecord.id);
                ModuleCommands.Add(new KeyValuePair<uint, string>(txRecord.id, tx));
            }
        }
    }
    
    private void LoadModules()
    {
        if (_database == null)
            return;

        SelectedModuleId = null;
        ModuleCommands?.Clear();

        if (Modules == null)
            Modules = new List<KeyValuePair<ushort, string>>();
        else
            Modules.Clear();

        try
        {
            for (ushort i = 0x0000; i < 0x2000; ++i)
            {
                var module = _database.getModule(i);
                if (string.IsNullOrEmpty(module))
                    continue;
                Modules.Add(new KeyValuePair<ushort, string>(i, module));
            }

            Modules.Sort((kv1, kv2) => string.Compare(kv1.Value, kv2.Value, StringComparison.Ordinal));
        }
        catch (Exception e)
        {            
            MessageBox.Show(e.Message,"Failed loading modules", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public void OnNavigateAway()
    {
        _database = null;
        IsDatabaseLoaded = false;
        
        Modules?.Clear();
        ModuleCommands?.Clear();
    }
}