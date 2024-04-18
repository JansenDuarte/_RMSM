
/*

        - Created by Jansen Duarte -

        Collection of a bunch of extensions gathered
        over a bunch of game development.

*/

namespace ExtensionMethods
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;
    using UnityEngine;


    /// <summary>
    /// Container for extension methods
    /// </summary>
    public static class ExM
    {



        #region ARRAY_EXTENSIONS

        /// <summary>
        /// Returns the inverted array
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        public static int[] Invert(this int[] _array)
        {
            int[] inverted = new int[_array.Length];

            for (int i = _array.Length; i > 0; i--) { inverted[_array.Length - i] = _array[i]; }

            return inverted;
        }

        /// <summary>
        /// Returns the inverted array
        /// </summary>
        /// <param name="_array"></param>
        /// <returns></returns>
        public static float[] Invert(this float[] _array)
        {
            float[] inverted = new float[_array.Length];

            for (int i = _array.Length; i > 0; i--) { inverted[_array.Length - i] = _array[i]; }

            return inverted;
        }

        #endregion // ARRAY_EXTENSIONS


        #region SPRITE_EXTENSION

        /// <summary>
        /// Load a country flag on Resources/00.FlagIcons/ + _countryName
        /// </summary>
        /// <param name="_countryName"></param>
        /// <returns></returns>
        public static Sprite Fetch_CountryFlag(string _countryName)
        {
            Sprite spt = Resources.Load<Sprite>("00.FlagIcons/" + _countryName);
            return spt;
        }

        #endregion // SPRITE_EXTENSION



        #region VECTOR_2_EXTENSIONS

        /// <summary>
        /// Generates random Vector2
        /// </summary>
        /// <param name="_limit">Value limit</param>
        /// <param name="_useNegativeNumbers"></param>
        /// <param name="_removeAxis"></param>
        /// <returns></returns>
        public static Vector2 RandomVector2(float _limit, bool _useNegativeNumbers = true, Axis _removeAxis = Axis.NILL)
        {
            Vector2 retValue;
            float x = _useNegativeNumbers ? Random.Range(-_limit, _limit) : Random.Range(0f, _limit);
            float y = _useNegativeNumbers ? Random.Range(-_limit, _limit) : Random.Range(0f, _limit);
            switch (_removeAxis)
            {
                case Axis.X:
                    x = 0f;
                    break;
                case Axis.Y:
                    y = 0f;
                    break;
                default:
                    break;
            }
            retValue = new(x, y);
            return retValue;
        }


        /// <summary>
        /// Add a value to every component of the vector
        /// </summary>
        /// <param name="_vector"></param>
        /// <param name="_value"></param>
        public static void AddScalar(this Vector2 _vector, float _value)
        {
            _vector.x += _value;
            _vector.y += _value;
        }

        /// <summary>
        /// Returns a vector with the value added to the selected axis
        /// </summary>
        /// <param name="_vector"></param>
        /// <param name="_value"></param>
        /// <param name="_axis"></param>
        /// <returns></returns>
        public static Vector2 AddScalar(Vector2 _vector, float _value, Axis _axis)
        {
            Vector2 retValue = _vector;
            retValue.AddScalar_OnAxis(_value, _axis);
            return retValue;
        }

        /// <summary>
        /// Adds a value to the selected axis
        /// </summary>
        /// <param name="_vector"></param>
        /// <param name="_value"></param>
        /// <param name="_axis"></param>
        public static void AddScalar_OnAxis(this ref Vector2 _vector, float _value, Axis _axis)
        {
            switch (_axis)
            {
                case Axis.X:
                    _vector.x += _value;
                    break;
                case Axis.Y:
                    _vector.y += _value;
                    break;
                default:
                    break;
            }
        }

        #endregion // VECTOR_2_EXTENSIONS



        #region VECTOR_3_EXTENSIONS

        /// <summary>
        /// Generates random Vector3
        /// </summary>
        /// <param name="_limit">Value limit</param>
        /// <param name="_useNegativeNumbers"></param>
        /// <param name="_removeAxis"></param>
        /// <returns></returns>
        public static Vector3 RandomVector3(float _limit, bool _useNegativeNumbers = true, Axis _removeAxis = Axis.NILL)
        {
            Vector3 retValue;
            float x = _useNegativeNumbers ? Random.Range(-_limit, _limit) : Random.Range(0f, _limit);
            float y = _useNegativeNumbers ? Random.Range(-_limit, _limit) : Random.Range(0f, _limit);
            float z = _useNegativeNumbers ? Random.Range(-_limit, _limit) : Random.Range(0f, _limit);
            switch (_removeAxis)
            {
                case Axis.X:
                    x = 0f;
                    break;
                case Axis.Y:
                    y = 0f;
                    break;
                case Axis.Z:
                    z = 0f;
                    break;
                default:
                    break;
            }
            retValue = new(x, y, z);
            return retValue;
        }

        /// <summary>
        /// Add a value to every component of the vector
        /// </summary>
        /// <param name="_vector"></param>
        /// <param name="_value"></param>
        public static void AddScalar(this Vector3 _vector, float _value)
        {
            _vector.x += _value;
            _vector.y += _value;
            _vector.z += _value;
        }

        /// <summary>
        /// Returns a vector with the value added to the selected axis
        /// </summary>
        /// <param name="_vector"></param>
        /// <param name="_value"></param>
        /// <param name="_axis"></param>
        /// <returns></returns>
        public static Vector3 AddScalar(Vector3 _vector, float _value, Axis _axis)
        {
            Vector3 retValue = _vector;
            retValue.AddScalar_OnAxis(_value, _axis);
            return retValue;
        }

        /// <summary>
        /// Adds a value to the selected axis
        /// </summary>
        /// <param name="_vector"></param>
        /// <param name="_value"></param>
        /// <param name="_axis"></param>
        public static void AddScalar_OnAxis(this ref Vector3 _vector, float _value, Axis _axis)
        {
            switch (_axis)
            {
                case Axis.X:
                    _vector.x += _value;
                    break;
                case Axis.Y:
                    _vector.y += _value;
                    break;
                case Axis.Z:
                    _vector.z += _value;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Returns vector that points towards "to"
        /// </summary>
        /// <param name="_from"></param>
        /// <param name="_to"></param>
        /// <returns></returns>
        public static Vector3 Direction(this Vector3 _from, Vector3 _to)
        {
            return _to - _from;
        }

        #endregion // VECTOR_3_EXTENSIONS



        #region FLOAT_EXTENTIONS

        /// <summary>
        /// Change the float to fit inside the new range of values
        /// </summary>
        /// <param name="_f"></param>
        /// <param name="_oldMin"></param>
        /// <param name="_oldMax"></param>
        /// <param name="_newMin"></param>
        /// <param name="_newMax"></param>
        public static void Change_Range(this float _f, float _oldMin, float _oldMax, float _newMin, float _newMax)
        {
            _f = (_f - _oldMin) / (_oldMax - _oldMin) * (_newMax - _newMin) + _newMin;
        }

        #endregion // FLOAT_EXTENTIONS



        #region INTEGER_EXTENSIONS

        /// <summary>
        /// Changes the int to fit inside the new range of values rounding down
        /// </summary>
        /// <param name="_i"></param>
        /// <param name="_oldMin"></param>
        /// <param name="_oldMax"></param>
        /// <param name="_newMin"></param>
        /// <param name="_newMax"></param>
        public static void Change_Range(this int _i, int _oldMin, int _oldMax, int _newMin, int _newMax)
        {
            _i = (_i - _oldMin) / (_oldMax - _oldMin) * (_newMax - _newMin) + _newMin;
        }

        #endregion // INTEGER_EXTENSIONS



        #region STRING_EXTENSIONS

        public static int[] StringIntoIndexes(this string _s)
        {
            string[] c = _s.Split(',');
            int[] indexes = new int[c.Length];

            for (int i = 0; i < indexes.Length; i++)
            {
                indexes[i] = int.Parse(c[i]);
            }

            return indexes;
        }


        /// <summary>
        /// Parse the curve to a string
        /// </summary>
        /// <param name="_curve"></param>
        /// <returns></returns>
        public static string Convert_TrackToString(this BezierCurve _curve)
        {
            BezierPoint[] points = _curve.GetAnchorPoints();
            string retValue = string.Empty;
            char style;

            for (int i = 0; i < _curve.pointCount; i++)
            {
                style = points[i].handleStyle.ToString()[0]; //first letter of the style
                retValue += string.Format("[{0},{1},{2},{3}];",
                points[i].localPosition,
                points[i].handle1,
                points[i].handle2,
                style);
            }

            Debug.Log(retValue);
            return retValue;
        }


        /// <summary>
        /// Turns a string in the format "RGBA(#.###, #.###, #.###, #.###)" into a Color
        /// </summary>
        /// <param name="_s"></param>
        /// <returns></returns>
        public static Color StringToColor(this string _s)
        {
            Color c = new();

            if (!_s.Contains("RGBA"))
                return c;

            char[] aux = { 'R', 'G', 'B', 'A', '(', ')' };
            _s = _s.Trim(aux);

            string[] colors = _s.Split(',');

            c.r = float.Parse(colors[0], CultureInfo.InvariantCulture);
            c.g = float.Parse(colors[1], CultureInfo.InvariantCulture);
            c.b = float.Parse(colors[2], CultureInfo.InvariantCulture);
            c.a = float.Parse(colors[3], CultureInfo.InvariantCulture);

            return c;
        }

        #endregion // STRING_EXTENSIONS



    }

    public enum Axis
    {
        NILL = -1,
        X = 0,
        Y = 1,
        Z = 2
    }
}