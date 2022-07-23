using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace nmxi.easylogview{
    public class EasyLogView : MonoBehaviour {
        [SerializeField]
        private Text m_textUI;
        [SerializeField] private Scrollbar _bar;
        private void Awake(){
            m_textUI = GetComponent<Text>();
            Application.logMessageReceived += OnLogMessage;
            
            m_textUI.text = String.Empty;
        }

        private void OnDestroy()
        {
            Application.logMessageReceived -= OnLogMessage;
        }
        
        private void OnLogMessage( string i_logText, string i_stackTrace, LogType type )
        {

            if (i_logText.Length >= 5)
                i_logText.Substring(1, 5);
            
            if (type != LogType.Error && type != LogType.Warning)
            {
                var tmp = m_textUI.text;
                if (tmp.Length >= 777)
                {
                    tmp = tmp.Substring(1, 555).ToString();
                    m_textUI.text = tmp;
                }

                m_textUI.text = "[" + DateTime.Now.ToLongTimeString() + "] ";
                m_textUI.text += i_logText + Environment.NewLine;
                
                m_textUI.text += tmp;
//                _bar.value = 1;
            }
            /*
            if (i >= 15)
            {
                m_textUI.text = "";
                i = 0;
            }
               
            i++;
            var tmp = m_textUI.text;
            //m_textUI.text = "[" + DateTime.Now.ToLongTimeString() + "] ";
            switch (type){
                case LogType.Warning:
                    //m_textUI.text += "<color=#ffff00>" + i_logText + "</color>" + Environment.NewLine;
                    break;
                case LogType.Error:
                    //m_textUI.text += "<color=#ff0000>" + i_logText + "</color>" + Environment.NewLine;
                    break;
                default:
                    m_textUI.text = "[" + DateTime.Now.ToLongTimeString() + "] ";
                    m_textUI.text += i_logText + Environment.NewLine;
                    break;
            }
            */
        }

        public void ResetLog(){
            m_textUI.text = "[" + DateTime.Now.ToLongTimeString() + "] Clear log";
        }
    }
}
