using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

namespace PatternMaker {
    public enum Operation { DoNothing, Toggle, Enable, Disable };

    public class PatternMakerAgent : Agent
    {
        public PatternController AgentPattern;
        public PatternController ExamplePattern;


        [Tooltip("Because we want an observation right before making a decision, we can force " + 
             "a camera to render before making a decision. Place the agentCam here if using " +
             "RenderTexture as observations.")]

        public Camera[] renderCameras;
        public float timeBetweenDecisionsAtInference = 0.15f;
        private PatternMakerAcademy academy;
        private float timeSinceDecision;
        private float pixelCount;

        private int PixelDifference { get {
            bool[] agentpixels = this.AgentPattern.GetPatternAsBooleans();
            bool[] examplepixels = this.ExamplePattern.GetPatternAsBooleans();

            int diff = 0;
            for (int i=0; i<examplepixels.Length; i++)
                if (examplepixels[i] != agentpixels[i])
                    diff -= 1;

            return diff;
        } }

        public override void InitializeAgent() {
            academy = FindObjectOfType(typeof(PatternMakerAcademy)) as PatternMakerAcademy;

            // SetSize also clears pattern
            this.AgentPattern.SetSize(this.ExamplePattern.GetSize());
            this.pixelCount = this.ExamplePattern.GetSize().x * this.ExamplePattern.GetSize().y;

            // Debug.Log("InitializeAgent");
        }

        public override void AgentReset() {
            // this.AgentPattern.ClearPattern();
            this.AgentPattern.SetSize(this.ExamplePattern.GetSize());
            this.pixelCount = this.ExamplePattern.GetSize().x * this.ExamplePattern.GetSize().y;

            // Debug.Log("AgentReset");
        }

        public override void CollectObservations()
        {
            // There are no numeric observations to collect as this environment uses visual
            // observations.
        }

        public override void AgentAction(float[] vectorAction, string textAction)
        {
            // Apply values in vectorAction to our scene;
            float actionX = Mathf.Clamp(vectorAction[0], 0, 1.0f);
            float actionY = Mathf.Clamp(vectorAction[1], 0, 1.0f);
            float actionOp = Mathf.Clamp(vectorAction[2], 0, 1.0f);

            Operation op;
            // if (actionOp >= 0.75f) op = Operation.Disable;
            // else if (actionOp >= 0.50f) op = Operation.Enable;
            if (actionOp >= 0.5f) op = Operation.Toggle;
            else op = Operation.DoNothing;


            int beforeDiff = this.PixelDifference;
            int afterDiff;

            if (op.Equals(Operation.Toggle)) {
                var patternSize = this.AgentPattern.GetSize();
                int ix = Mathf.FloorToInt((patternSize.x-1) * actionX);
                int iy = Mathf.FloorToInt((patternSize.y-1) * actionY);
                int idx = patternSize.x * iy + ix;

                if (idx < 0 || idx > this.pixelCount) {
                    Debug.Log("Invalid pixel index: "+idx.ToString());
                }
                else {
                    // Perform pixel toggle
                    this.AgentPattern.Toggle(idx);
                }
                
                afterDiff = this.PixelDifference;
            } else {
                afterDiff = beforeDiff;
            }

            // Calculate reward
            float reward = 0.0f;
            if (afterDiff > beforeDiff) reward = -1f;
            if (afterDiff < beforeDiff) reward = 1f;
            reward -= afterDiff / pixelCount * 0.1f;
            SetReward(reward);
            if (afterDiff == 0) Done();


            // if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
            // {
            //     var actionZ = 2f * Mathf.Clamp(vectorAction[0], -1f, 1f);
            //     var actionX = 2f * Mathf.Clamp(vectorAction[1], -1f, 1f);

            //     if ((gameObject.transform.rotation.z < 0.25f && actionZ > 0f) ||
            //         (gameObject.transform.rotation.z > -0.25f && actionZ < 0f))
            //     {
            //         gameObject.transform.Rotate(new Vector3(0, 0, 1), actionZ);
            //     }

            //     if ((gameObject.transform.rotation.x < 0.25f && actionX > 0f) ||
            //         (gameObject.transform.rotation.x > -0.25f && actionX < 0f))
            //     {
            //         gameObject.transform.Rotate(new Vector3(1, 0, 0), actionX);
            //     }
            // }

            // GridAgent
            // AddReward(-0.01f);
            // int action = Mathf.FloorToInt(vectorAction[0]);

            // Vector3 targetPos = transform.position;
            // switch (action)
            // {
            //     case NoAction:
            //         // do nothing
            //         break;
            //     case Right:
            //         targetPos = transform.position + new Vector3(1f, 0, 0f);
            //         break;
            //     case Left:
            //         targetPos = transform.position + new Vector3(-1f, 0, 0f);
            //         break;
            //     case Up:
            //         targetPos = transform.position + new Vector3(0f, 0, 1f);
            //         break;
            //     case Down:
            //         targetPos = transform.position + new Vector3(0f, 0, -1f);
            //         break;
            //     default:
            //         throw new ArgumentException("Invalid action value");
            // }

            // Collider[] blockTest = Physics.OverlapBox(targetPos, new Vector3(0.3f, 0.3f, 0.3f));
            // if (blockTest.Where(col => col.gameObject.CompareTag("wall")).ToArray().Length == 0)
            // {
            //     transform.position = targetPos;

            //     if (blockTest.Where(col => col.gameObject.CompareTag("goal")).ToArray().Length == 1)
            //     {
            //         Done();
            //         SetReward(1f);
            //     }
            //     if (blockTest.Where(col => col.gameObject.CompareTag("pit")).ToArray().Length == 1)
            //     {
            //         Done();
            //         SetReward(-1f);
            //     }
            // }
        }

        public void FixedUpdate()
        {
            WaitTimeInference();
        }

        private void WaitTimeInference()
        {
            foreach(var cam in renderCameras)
                cam.Render();

            if (!academy.GetIsInference())
            {
                RequestDecision();
            }
            else
            {
                if (timeSinceDecision >= timeBetweenDecisionsAtInference)
                {
                    timeSinceDecision = 0f;
                    RequestDecision();
                }
                else
                {
                    timeSinceDecision += Time.fixedDeltaTime;
                }
            }
        }
    }
}