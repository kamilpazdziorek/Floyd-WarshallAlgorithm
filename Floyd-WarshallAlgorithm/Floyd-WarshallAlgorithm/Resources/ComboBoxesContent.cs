using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Floyd_WarshallAlgorithm.Resources
{
    public static class CitiesExamples
    {
        public static ObservableCollection<string> Cities = new ObservableCollection<string> {
                "Gdańsk",
                "Gdynia",
                "Szczecin",
                "Olsztyn",
                "Łódź",
                "Kraków",
                "Poznań",
                "Warszawa",
                "Wrocław",
                "Kielce",
                "Opole",
                "Lublin",
                "Bydgoszcz",
                "Katowice",
                "Rzeszów" };
    }

    public class NumbersExamples
    {
        public static ObservableCollection<int> numbers = new ObservableCollection<int>
        {
            0,1,2,3,4,5,6,7,8,9,10,15,20,25,30,40,50,60,70,80,90,100
        };
    }
}
