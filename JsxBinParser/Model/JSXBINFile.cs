using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;

namespace JsxBinParser.Model
{
    [Serializable]
    public sealed class JSXBINFile : INotifyPropertyChanged
    {
        public enum ParseStatus
        {
            Init,
            Error,
            Success,
            Solving
        }

        private bool _isChecked;
        private ParseStatus _status;
        private bool _printtreestructure;

        public int Index
        {
            get;
            set;
        }

        public ParseStatus Status
        {
            get => _status;
            set
            {
                if (_status != value)
                {
                    _status = value;
                    OnPropertyChanged(nameof(Status));
                    OnPropertyChanged(nameof(Image));
                }
            }
        }

        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(nameof(IsChecked));
                }
            }
        }

        public string FileName
        {
            get;
            set;
        }

        public string FilePath
        {
            get;
            set;
        }

        public bool PrintTreeStructure
        {
            get => _printtreestructure;
            set
            {
                if (_printtreestructure != value)
                {
                    _printtreestructure = value;
                    OnPropertyChanged(nameof(PrintTreeStructure));
                }
            }
        }

        public string TreeStructure
        {
            get;
            set;
        }

        public List<Parser.Model.TreeStructure> Structures
        {
            get;
            set;
        }

        public string ErrorMessage
        {
            get;
            set;
        }

        public Image Image
        {
            get
            {
                switch (Status)
                {
                    case ParseStatus.Success: return ResImage.Success;
                    case ParseStatus.Error: return ResImage.Error;
                    case ParseStatus.Solving: return ResImage.Solve;
                    default: return ResImage.Init;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
