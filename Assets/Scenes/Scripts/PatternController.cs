using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PatternMaker {
    public class PatternController : MonoBehaviour
    {
        [Header("Prefabs")]
        public RectTransform PixelPrefab;

        private Vector2Int PatternSize;
        private RectTransform[] Pixels;

        private void Start() {
            this.Pixels = new RectTransform[0];
            this.PatternSize = new Vector2Int(0,0);
            this.PixelPrefab.gameObject.SetActive(false);
        }

        public void SetSize(int w, int h) {
            this.ClearPattern();

            this.PatternSize = new Vector2Int(w,h);
            this.Pixels = new RectTransform[w*h];
            for(int i=0; i<w*h; i++) this.Pixels[i] = null;
        }

        public void SetSize(Vector2Int dimm) {
            this.SetSize(dimm.x, dimm.y);
        }

        public void RandomPattern(int count) {
            var availableIndices = new List<int>();
            for (int i=this.PatternSize.x * this.PatternSize.y-1; i>=0; i--)
                availableIndices.Add(i);

            if (count > availableIndices.Count) {
                Debug.Log("PatternController.RandomPattern received count value of "+count.ToString()+" which is too much for the current PatternSize ("+this.PatternSize.ToString()+")");
                return;
            }

            // load random selection of indices into selectedIndices
            var selectedIndices = new int[count];
            var rnd = new System.Random();
            for (int i=0; i< count; i++) {
                int idx = rnd.Next(availableIndices.Count);
                selectedIndices[i] = availableIndices[idx];
                availableIndices.RemoveAt(idx);
            }

            // clear pattern
            this.ClearPattern();
            // create pixels for our randomly generated pattern
            foreach(var idx in selectedIndices)
                this.Toggle(idx);
        }

        public void ClearPattern() {
            foreach(var pix in this.Pixels)
                if (pix != null)
                    Destroy(pix.gameObject);
        }

        private void Toggle(int pixelIndex) {
            if (this.Pixels[pixelIndex] != null) { // pixel is ON?
                // turn it OFF
                Destroy(this.Pixels[pixelIndex].gameObject);
                this.Pixels[pixelIndex] = null;
                return;
            }

            // Pixel is OFF, turn it ON (spawn pixel)
            var pix = this.CreatePixel(pixelIndex);
            this.Pixels[pixelIndex] = pix;
        }

        private RectTransform CreatePixel(int idx) {
            var go = Instantiate(this.PixelPrefab.gameObject, this.PixelPrefab.parent);
            var rt = go.GetComponent<RectTransform>();
            int y = (int)Math.Floor(((float)idx / (float)this.PatternSize.x));
            int x = idx - y * this.PatternSize.x;
            ConfigPixel(rt, x, y);
            go.SetActive(true);
            return rt;
        }

        private void ConfigPixel(RectTransform rt, int x, int y) {
            var parent = rt.parent.GetComponent<RectTransform>();
            // Debug.Log("Parent size: "+parent.sizeDelta.ToString());
            var size = new Vector2(parent.sizeDelta.x / this.PatternSize.x, parent.sizeDelta.y / this.PatternSize.y);
            rt.sizeDelta = size;
            rt.anchoredPosition += new Vector2(size.x * x, -size.y * y);
        }
    }
}