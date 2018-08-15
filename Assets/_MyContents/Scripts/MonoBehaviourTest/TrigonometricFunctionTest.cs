namespace MainContents
{
    using System.Text;
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    /// <summary>
    /// MonoBehaviourで回すテスト
    /// </summary>
    public sealed class TrigonometricFunctionTest : MonoBehaviour
    {
        const int Count = 1000000;

        void Start()
        {
            var sw = new System.Diagnostics.Stopwatch();

            // ---------------------------------------
            // 普通に計算
            sw.Restart();
            for (int i = 0; i < Count; ++i)
            {
                float rad = i * Mathf.Deg2Rad;
                Mathf.Sin(rad);
                Mathf.Cos(rad);
            }
            sw.Stop();
            Debug.Log(" --- Mathf : " + sw.Elapsed);

            // ---------------------------------------
            // PureLUI
            const int _tableLength = 360;
            float[] _sinTable = new float[_tableLength];
            float[] _cosTable = new float[_tableLength];
            for (int i = 0; i < TableLength; ++i)
            {
                _sinTable[i] = Mathf.Sin(i * Mathf.Deg2Rad);
                _cosTable[i] = Mathf.Cos(i * Mathf.Deg2Rad);
            }
            float ret = 0;
            sw.Restart();
            for (int i = 0; i < Count; ++i)
            {
                int deg = i % 360;
                ret = _sinTable[deg];
                ret = _cosTable[deg];
            }
            sw.Stop();
            Debug.Log(" --- PureLUI : " + sw.Elapsed);

            // ---------------------------------------
            // LUI
            for (int i = 0; i <= TableLength; ++i)
            {
                SinTable[i] = Mathf.Sin(i * Mathf.Deg2Rad);
            }
            sw.Restart();
            for (int i = 0; i < Count; ++i)
            {
                int deg = i % 360;
                Sin_T(deg);
                Cos_T(deg);
            }
            sw.Stop();
            Debug.Log(" --- LUT : " + sw.Elapsed);

            // ---------------------------------------
            // マクローリン展開
            sw.Restart();
            for (int i = 0; i < Count; ++i)
            {
                MaclaurinSin(i);
                MaclaurinCos(i);
            }
            sw.Stop();
            Debug.Log(" --- Maclaurin : " + sw.Elapsed);
        }


        // ----------------------------------------------------
        #region // LUT
        const int TableLength = 90;
        float[] SinTable = new float[TableLength + 1];

        float Sin_T(int deg)
        {
            deg = deg % 360;
            // 0~90(度)
            if (deg <= 90) { return this.SinTable[deg]; }
            // 90~180
            else if (deg <= 180) { return this.SinTable[180 - deg]; }
            // 180~270
            else if (deg <= 270) { return -this.SinTable[deg - 180]; }
            // 270~360
            else { return -this.SinTable[360 - deg]; }
        }

        float Cos_T(int deg)
        {
            return Sin_T(deg + 90);
        }

        #endregion // LUT


        // ----------------------------------------------------
        #region // Maclaurin

        float MaclaurinSin(int deg)
        {
            deg = deg % 360;
            int sign = 1;
            // 0~90(度)
            if (deg <= 90) { }
            // 90~180
            else if (deg <= 180) { deg = 180 - deg; }
            // 180~270
            else if (deg <= 270) { sign = -1; deg = deg - 180; }
            // 270~360
            else { sign = -1; deg = 360 - deg; }

            var rad = deg * Mathf.Deg2Rad;
            float pow2 = rad * rad;
            float pow3 = pow2 * rad;    // x^3
            float pow5 = pow3 * pow2;   // x^5
            float pow7 = pow5 * pow2;   // x^7
            float pow9 = pow7 * pow2;   // x^7

            // 階乗は算出コストを省くために数値リテラルで持つ
            float ret = rad - (pow3 / 6f)   // 3!
                        + (pow5 / 120f)     // 5!
                        - (pow7 / 5040f)    // 7!
                        + (pow9 / 362880f); // 9!
            return ret * sign;
        }

        float MaclaurinCos(int deg)
        {
            return MaclaurinSin(deg + 90);
        }

        #endregion // Maclaurin
    }
}
