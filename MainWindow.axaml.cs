using Avalonia.Controls;
using Avalonia.Interactivity;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;

namespace TodoListApp
{
    public partial class MainWindow : Window
    {
        private ObservableCollection<TodoItem> _tasks;

        public MainWindow()
        {
            InitializeComponent();
            _tasks = new ObservableCollection<TodoItem>();
            TasksList.ItemsSource = _tasks;
            
            // Subscribe to collection changes to update stats
            _tasks.CollectionChanged += (s, e) => UpdateStats();
            
            UpdateStats();
        }

        private void AddTask_Click(object? sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(TaskInput.Text))
            {
                var newTask = new TodoItem
                {
                    Description = TaskInput.Text,
                    IsCompleted = false,
                    CreatedAt = DateTime.Now
                };
                
                // Subscribe to property changes to update stats when checkbox is toggled
                newTask.PropertyChanged += (s, e) => UpdateStats();
                
                _tasks.Add(newTask);
                TaskInput.Text = string.Empty;
                TaskInput.Focus();
                UpdateStats();
            }
        }

        private void DeleteTask_Click(object? sender, RoutedEventArgs e)
        {
            if (sender is Button button && button.Tag is TodoItem task)
            {
                _tasks.Remove(task);
                UpdateStats();
            }
        }

        private void ClearCompleted_Click(object? sender, RoutedEventArgs e)
        {
            var completedTasks = _tasks.Where(t => t.IsCompleted).ToList();
            foreach (var task in completedTasks)
            {
                _tasks.Remove(task);
            }
            UpdateStats();
        }

        private void UpdateStats()
        {
            TotalTasksText.Text = $"Total: {_tasks.Count}";
            CompletedTasksText.Text = $"Completed: {_tasks.Count(t => t.IsCompleted)}";
        }
    }

    public class TodoItem : INotifyPropertyChanged
    {
        private bool _isCompleted;
        private string _description = string.Empty;
        
        public string Description 
        { 
            get => _description;
            set
            {
                _description = value;
                OnPropertyChanged();
            }
        }
        
        public bool IsCompleted 
        { 
            get => _isCompleted;
            set
            {
                _isCompleted = value;
                OnPropertyChanged();
            }
        }
        
        public DateTime CreatedAt { get; set; }

        public event PropertyChangedEventHandler? PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}