using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace ModelPolosin
{
    internal class DrawTable
    {
        private readonly List<double> _listOfChannelLength;
        private readonly List<double> _listOfTemperatures;
        private readonly List<double> _listOfViscosity;
        private readonly DataGrid _table;

        public DrawTable(
            List<double> listOfChannelLength,
            List<double> listOfTemperatures,
            List<double> listOfViscosity,
            DataGrid table)
        {
            _listOfChannelLength = listOfChannelLength;
            _listOfTemperatures = listOfTemperatures;
            _listOfViscosity = listOfViscosity;
            _table = table;
            MakeTable();
        }

        private void MakeTable()
        {
            List<DataForTable> data = new();
            for (var i = 0; i < _listOfChannelLength.Count; i++)
                data.Add(new DataForTable
                {
                    Coordinate = _listOfChannelLength[i],
                    Temperature = _listOfTemperatures[i],
                    Viscosity = _listOfViscosity[i]
                });
            _table.ItemsSource = data;
        }


        private class DataForTable
        {
            public double Coordinate { get; set; }
            public double Temperature { get; set; }
            public double Viscosity { get; set; }
        }
    }
}
