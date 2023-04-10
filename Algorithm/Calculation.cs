namespace Algorithm {
    public class Calculation {
        public double _R = 8.314;
        public double _W = 0.12;
        public double _H = 0.02;
        public double _L = 7.5;
        public double _ro = 950;
        public double _c = 2250;
        public double _T0 = 120;
        public double _Vu = 0.5;
        public double _Tu = 205;
        public double _m0 = 29940;
        public double _Tr = 190;
        public double _n = 0.35;
        public double _alphaU = 425;
        public double _Ea = 30000;
        public double _step = 0.1;

        public double _gammaPoint = 0;
        public double _qGamma = 0;
        public double _beta = 0;
        public double _qAlpha = 0;
        public double _F = 0;
        public double _Qch = 0;
        public double _z = 0;
        public int _Q = 0;

        public void InitializingVariables() {
            // скорость деформации сдвига
            _gammaPoint = _Vu / _H;

            // удельные тепловые потоки
            _qGamma = _H * _W * _m0 * Math.Pow(_gammaPoint, _n + 1);
            _beta = _Ea / (_R * (_T0 + 20 + 273) * (_Tr + 273));
            _qAlpha = _W * _alphaU * (Math.Pow(_beta, -1) - _Tu + _Tr);

            // коэффициент геометрической формы канала 
            _F = 0.125 * Math.Pow(_H / _W, 2) - 0.625 * _H / _W + 1;

            // объемный расход потока материала в канале 
            _Qch = _H * _W * _Vu / 2 * _F;

        }

        // температура материала в канале
        public double Temperature(double z) {
            return Math.Round(_Tr + 1 / _beta * Math.Log(_beta * _qGamma + _W * _alphaU / (_beta * _qAlpha) * (1 - Math.Exp(-(_beta * _qAlpha / (_ro * _c * _Qch) * z) + Math.Exp(_beta * (_T0 - _Tr - _qAlpha / (_ro * _c * _Qch) * z))))), 5);
        }

        // вязкость материала в канале
        public double Viscosity(double T) {
            return Math.Round(_m0 * Math.Exp(-_beta * (T - _Tr)) * Math.Pow(_gammaPoint, _n + 1), 5);
        }

        // производительность канала
        public double Effiency() {
            return Math.Round(_ro * _Qch, 5);
        }

        // список координат по длине канала для таблицы
        public List<double> ListOfChannelLength() {
            List<double> coordinates = new();
            for (double i = 0; i <= _L; i += _step) {
                coordinates.Add(i);
            }
            return coordinates;
        }

        // список температур для таблицы
        public List<double> ListOfTemperatures(List<double> coordinates) {
            List<double> listOfTemperatures = new();
            foreach (double coordinate in coordinates) {
                listOfTemperatures.Add(Temperature(coordinate));
            }
            return listOfTemperatures;
        }

        // список вязкости для таблицы
        public List<double> ListOfViscosity(List<double> listOfTemperatures) {
            List<double> listOfViscosity = new();
            foreach (double temp in listOfTemperatures) {
                listOfViscosity.Add(Temperature(temp));
            }
            return listOfViscosity;
        }
    }
}