using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using System;
using System.Globalization;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace AccStandardDeviationCounter
{
    public class AccStandardDeviationCounter:BasicCustomCounter,INoteEventHandler
    {
        private int _noteLeft = 0;
        private int _noteRight = 0;

        private double _averageLeft;
        private double _averageRight;

        private double _totalLeft = 0;
        private double _totalRight = 0;
        private double _averageBoth;

        private List<double> _listLeft = new List<double>();
        private List<double> _listRight = new List<double>();
        private List<double> _listBoth = new List<double>();

        private double _standardDeviationLeft=0;
        private double _standardDeviationRight=0;
        private double _standardDeviationBoth;

        private TMP_Text _counterLeft;
        private TMP_Text _counterRight;

        float x = Configuration.Instance.OffsetX;
        float y = Configuration.Instance.OffsetY;
        float z = Configuration.Instance.OffsetZ;

        public override void CounterInit()
        {
            string defaultValue = Format(0, Configuration.Instance.DecimalPrecision);

            if (Configuration.Instance.EnableLabel)
            {
                var label = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(x, y, z));
                label.text = "Acc Standard Deviation";
                label.fontSize = Configuration.Instance.LabelFontSize;
            }

            Vector3 leftOffset = new Vector3(x, y - 0.2f,z);
            TextAlignmentOptions leftAlign = TextAlignmentOptions.Top;
            if (Configuration.Instance.SeparateSaber)
            {
                _counterRight = CanvasUtility.CreateTextFromSettings(Settings, new Vector3(x + 0.2f, y - 0.2f, z));
                _counterRight.lineSpacing = -26;
                _counterRight.fontSize = Configuration.Instance.FigureFontSize;
                _counterRight.text = defaultValue;
                _counterRight.alignment = TextAlignmentOptions.TopLeft;

                leftOffset = new Vector3(x - 0.2f, y - 0.2f, z);
                leftAlign = TextAlignmentOptions.TopRight;
            }

            _counterLeft = CanvasUtility.CreateTextFromSettings(Settings, leftOffset);
            _counterLeft.lineSpacing = -26;
            _counterLeft.fontSize=Configuration.Instance.FigureFontSize;
            _counterLeft.text = defaultValue;
            _counterLeft.alignment = leftAlign;           
        }

        public void OnNoteMiss(NoteData data) { }

        public void OnNoteCut(NoteData data,NoteCutInfo info)
        {
            if (data.colorType == ColorType.None || !info.allIsOK) return;
            UpdateText(info.swingRatingCounter, info.cutDistanceToCenter, info.saberType);
        }

        public void UpdateText(ISaberSwingRatingCounter v,float distanceToCenter,SaberType saberType)
        {
            ScoreModel.RawScoreWithoutMultiplier(v, distanceToCenter, out int beforecut, out int aftercut, out int acc);

            _listBoth.Add(acc);

            if (saberType == SaberType.SaberA)
            {
                _listLeft.Add(acc);
                _totalLeft += acc;
                _noteLeft++;
                _averageLeft = _totalLeft / _noteLeft;
                _standardDeviationLeft = StandatdDeviation(_listLeft, _averageLeft, _noteLeft);
            }
            else
            {
                _listRight.Add(acc);
                _totalRight += acc;
                _noteRight++;
                _averageRight = _totalRight / _noteRight;
                _standardDeviationRight = StandatdDeviation(_listRight, _averageRight, _noteRight);
            }

            _averageBoth = (_totalLeft + _totalRight) / (_noteLeft + _noteRight);
            _standardDeviationBoth = StandatdDeviation(_listBoth, _averageBoth, (_noteLeft + _noteRight));
            
            UpdateText();
        }

        private void UpdateText()
        {
            if (Configuration.Instance.SeparateSaber)
            {
                _counterLeft.text = Format(_standardDeviationLeft, Configuration.Instance.DecimalPrecision);
                _counterRight.text = Format(_standardDeviationRight, Configuration.Instance.DecimalPrecision);
            }
            else _counterLeft.text = Format(_standardDeviationBoth, Configuration.Instance.DecimalPrecision);
        }

        private string Format(double StandardDeviation,int DecimalPrecision)
        {
            return StandardDeviation.ToString($"F{DecimalPrecision}", CultureInfo.InvariantCulture);
        }

        private double StandatdDeviation(List<double> AccList,double average,int notes)
        {
            double beforeDividedTotal = 0;

            foreach(double acc in AccList)
            {
                beforeDividedTotal += Math.Pow(acc - average, 2);
            }

            return Math.Sqrt(beforeDividedTotal / notes);
        }

        public override void CounterDestroy(){}
    }
}
