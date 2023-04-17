namespace Algorithm;

public class Calculation
{
    private readonly double _alphaU = 425;
    private readonly double _c = 2250;
    private readonly double _Ea = 30000;
    private readonly double _H = 0.02;
    public readonly double _L = 7.5;
    private readonly double _m0 = 29940;
    private readonly double _n = 0.35;
    private readonly double _R = 8.314;
    private readonly double _ro = 950;
    private readonly double _step = 0.1;
    private readonly double _T0 = 120;
    private readonly double _Tr = 190;
    private readonly double _Tu = 205;
    private readonly double _Vu = 0.5;
    private readonly double _W = 0.12;
    private double _beta;
    private double _F;
    private double _gammaPoint;
    private int _Q = 0;
    private double _qAlpha;
    private double _Qch;
    private double _qGamma;
    private double _z = 0;

    // public Calculation(double alphaU)
    // {
    //     _alphaU = alphaU;
    // }
    public void InitializingVariables()
    {
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
    public double Temperature(double z)
    {
         return _Tr + (1 / _beta) * Math.Log((_beta * _qGamma + _W * _alphaU) / (_beta * _qAlpha) * (1 - Math.Exp((-_beta * _qAlpha * z) / (_ro * _c * _Qch)))
+ Math.Exp(_beta * (_T0 - _Tr - (_qAlpha * z) / (_ro * _c * _Qch))));
    }

    // вязкость материала в канале
    public double Viscosity(double T)
    {
        return Math.Round(_m0 * Math.Exp(-_beta * (T - _Tr)) * Math.Pow(_gammaPoint, _n - 1), 2);
    }

    // производительность канала
    public double Effiency()
    {
        return Math.Round(_ro * _Qch, 2);
    }

    // список координат по длине канала для таблицы
    public List<double> ListOfChannelLength()
    {
        List<double> coordinates = new();
        for (var i = 0m; i <= (decimal)_L; i += (decimal)_step) coordinates.Add((double)i);
        return coordinates;
    }

    // список температур для таблицы
    public List<double> ListOfTemperatures(List<double> coordinates)
    {
        List<double> listOfTemperatures = new();
        foreach (var coordinate in coordinates)
            listOfTemperatures.Add(Math.Round(Temperature(coordinate), 5));
        return listOfTemperatures;
    }

    // список вязкости для таблицы
    public List<double> ListOfViscosity(List<double> listOfTemperatures)
    {
        return listOfTemperatures.Select(Viscosity).ToList();
    }
}