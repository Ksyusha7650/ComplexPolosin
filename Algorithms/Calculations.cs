using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Algorithms {
    public class Calculations {
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

        public double _gammaPoint = 0;
        public double _qGamma = 0;
        public double _beta = 0;
        public double _qAlpha = 0;
        public double _F = 0;
        public double _Qch = 0;
        public double _z = 0;
        public int _Q = 0;

        public void InitializingVariables() {
            // удельные тепловые потоки
            _qGamma = _H * _W * _m0 * Math.Pow(_gammaPoint, _n + 1);
            _beta = _Ea / (_R * (_T0 + 20 + 273) * (_Tr + 273));
            _qAlpha = _W * _alphaU * (Math.Pow(_beta, -1) - _Tu + _Tr);

            // скорость деформации сдвига
            _gammaPoint = _Vu / _H;

            // коэффициент геометрической формы канала 
            _F = 0.125 * Math.Pow(_H / _W, 2) - 0.625 * _H / _W + 1;

            // объемный расход потока материала в канале 
            _Qch = _H * _W * _Vu / 2 * _F;

        }

        // температура материала в канале
        public double Temperature(double z) {
            return _Tr + 1 / _beta * Math.Log(_beta * _qGamma + _W * _alphaU / (_beta * _qAlpha) * (1 - Math.Exp(-(_beta * _qAlpha / (_ro * _c * _Qch) * z) + Math.Exp(_beta * (_T0 - _Tr - _qAlpha / (_ro * _c * _Qch) * z)))));
        }

        // вязкость материала в канале
        public double Viscosity(double T) {
            return _m0 * Math.Exp(-_beta * (T - _Tr)) * Math.Pow(_gammaPoint, _n + 1);
        }

        // производительность канала
        public double Effiency() {
            return _ro * _Qch;
        }
    }
}
