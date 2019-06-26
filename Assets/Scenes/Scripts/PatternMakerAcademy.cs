using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace PatternMaker {
    public class PatternMakerAcademy : Academy
    {
        private int PatternWidth;
        private int PatternHeight;

        public override void InitializeAcademy() {
            PatternWidth = (int)base.resetParameters["PatternWidth"];
            PatternWidth = (int)base.resetParameters["PatternHeight"];
        }

        public override void AcademyReset() {

        }

        public override void AcademyStep() {

        }
    }
}