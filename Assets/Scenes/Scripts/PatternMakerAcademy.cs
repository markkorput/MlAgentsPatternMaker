using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace PatternMaker {
    public class PatternMakerAcademy : Academy
    {
        private int PatternWidth;
        private int PatternHeight;
        private int PatternCount;

        public PatternController ExamplePattern;
        public PatternController AgentPattern;

        public override void InitializeAcademy() {
            PatternWidth = (int)base.resetParameters["PatternWidth"];
            PatternHeight = (int)base.resetParameters["PatternHeight"];
            PatternCount = (int)base.resetParameters["PatternCount"];
        }

        public override void AcademyReset() {
            this.ExamplePattern.SetSize(this.PatternWidth, this.PatternHeight);
            this.AgentPattern.SetSize(this.PatternWidth, this.PatternHeight);

            this.ExamplePattern.RandomPattern(this.PatternCount);
            this.AgentPattern.ClearPattern();
        }

        public override void AcademyStep() {

        }
    }
}