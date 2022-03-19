using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using CountersPlus.Counters.Custom;
using CountersPlus.Counters.Interfaces;
using TMPro;
using UnityEngine;
using Zenject;

namespace AccStandardDeviationCounter
{
    public class AccStandardDeviationCounter : BasicCustomCounter, INoteEventHandler
    {
        [Inject] ScoreController scoreController;

        private double _averageLeft;
        private double _averageRight;
        private double _averageBoth;

        private double _totalLeft = 0;
        private double _totalRight = 0;

        private List<double> _listLeft = new List<double>();
        private List<double> _listRight = new List<double>();
        private List<double> _listBoth = new List<double>();

        private double _standardDeviationLeft = 0;
        private double _standardDeviationRight = 0;
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

            Vector3 leftOffset = new Vector3(x, y - 0.2f, z);
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
            _counterLeft.fontSize = Configuration.Instance.FigureFontSize;
            _counterLeft.text = defaultValue;
            _counterLeft.alignment = leftAlign;

            scoreController.scoringForNoteFinishedEvent += ScoreController_scoringForNoteFinishedEvent;
        }

        public void OnNoteMiss(NoteData data) { }

        public void OnNoteCut(NoteData data, NoteCutInfo info) { }

        private void ScoreController_scoringForNoteFinishedEvent(ScoringElement scoringElement)
        {
            if (scoringElement is GoodCutScoringElement goodCut)
            {
                var cutScoreBuffer = goodCut.cutScoreBuffer;

                var beforeCut = cutScoreBuffer.beforeCutScore;
                var afterCut = cutScoreBuffer.afterCutScore;
                var cutDistance = cutScoreBuffer.centerDistanceCutScore;
                var fixedScore = cutScoreBuffer.noteScoreDefinition.fixedCutScore;

                if (Configuration.Instance.SeparateSaber)
                {
                    var totalAccForHand = goodCut.noteData.colorType == ColorType.ColorA
                        ? _listLeft
                        : _listRight;

                    double averageAcc = goodCut.noteData.colorType == ColorType.ColorA
                        ? _averageLeft
                        : _averageRight;

                    /*
                    これは変数の更新はできないっぽい
                    double standardDeviation= goodCut.noteData.colorType == ColorType.ColorA
                        ? _standardDeviationLeft
                        : _standardDeviationRight;
                    */

                    switch (goodCut.noteData.scoringType)
                    {
                        case NoteData.ScoringType.Normal:
                            totalAccForHand.Add(cutDistance);
                            _listBoth.Add(cutDistance);
                            break;
                        case NoteData.ScoringType.BurstSliderHead when Configuration.Instance.IncludeChains:
                            totalAccForHand.Add(cutDistance);
                            _listBoth.Add(cutDistance);
                            break;

                            // Chain links are not being tracked at all because they give a fixed 20 score for every hit.
                            /*case NoteData.ScoringType.BurstSliderElement when Settings.IncludeChains:
                                totalScoresForHand[2] += fixedScore;
                                cutCountForHand[2]++;
                                break;*/


                    }

                    averageAcc = totalAccForHand.Sum() / totalAccForHand.Count;
                    if (goodCut.noteData.colorType == ColorType.ColorA)
                    {
                        _standardDeviationLeft = StandatdDeviation(totalAccForHand, averageAcc, totalAccForHand.Count);
                    }
                    else
                    {
                        _standardDeviationRight = StandatdDeviation(totalAccForHand, averageAcc, totalAccForHand.Count);
                    }

                    UpdateText();
                }
                else
                {
                    switch (goodCut.noteData.scoringType)
                    {
                        case NoteData.ScoringType.Normal:
                            _listBoth.Add(cutDistance);
                            break;
                        case NoteData.ScoringType.BurstSliderHead when Configuration.Instance.IncludeChains:
                            _listBoth.Add(cutDistance);
                            break;

                            // Chain links are not being tracked at all because they give a fixed 20 score for every hit.
                            /*case NoteData.ScoringType.BurstSliderElement when Settings.IncludeChains:
                                totalScoresForHand[2] += fixedScore;
                                cutCountForHand[2]++;
                                break;*/


                    }


                    _averageBoth = _listBoth.Sum() / _listBoth.Count;
                    _standardDeviationBoth = StandatdDeviation(_listBoth, _averageBoth, _listBoth.Count);

                    UpdateText();
                }
            }
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

        private string Format(double StandardDeviation, int DecimalPrecision)
        {
            return StandardDeviation.ToString($"F{DecimalPrecision}", CultureInfo.InvariantCulture);
        }

        private double StandatdDeviation(List<double> AccList, double average, int notes)
        {
            double beforeDividedTotal = 0;

            foreach (double acc in AccList)
            {
                beforeDividedTotal += Math.Pow(acc - average, 2);
            }

            return Math.Sqrt(beforeDividedTotal / notes);
        }

        public override void CounterDestroy() { }
    }
}
