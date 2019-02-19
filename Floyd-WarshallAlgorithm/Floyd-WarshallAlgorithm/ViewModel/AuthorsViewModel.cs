
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Floyd_WarshallAlgorithm.ViewModel
{
    class AuthorsViewModel
    {
        public ICommand CloseCommand => new RelayCommand(Close);

        private void Close(object obj)
        {
            (obj as Window).Close();
        }

    }
}
