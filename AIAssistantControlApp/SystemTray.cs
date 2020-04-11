using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Threading;
using Timer = System.Windows.Forms.Timer;

namespace AIAssistantControlApp
{
    class SystemTray
    {
        private NotifyIcon SysTrayIcon;
        private Timer _mAnimTimer;
        private int _animationIndex = 0;
        private bool _bIsTimeOut = false;
        private Icon [] _animIconList;
        private Dictionary<ServiceState, string> _iconSets;
        private bool bAnimate = false;
        public event EventHandler DoubleClick;
        public event EventHandler Click;
        public event EventHandler BalloonTipClicked;
        public event EventHandler BalloonTipClosed;
        public event EventHandler BalloonTipShown;
        public event MouseEventHandler MouseUp;
        public event MouseEventHandler MouseMove;
        public event MouseEventHandler MouseDown;
        public event MouseEventHandler MouseDoubleClick;
        public event MouseEventHandler MouseClick;
        

        public SystemTray()
        {
            InitDefaults();
        }
        public SystemTray(string pTrayText)
        {
            InitDefaults();
            SysTrayIcon.Text = pTrayText;
        }

        public SystemTray(string pTrayText, Icon pIcon)
        {
            InitDefaults();
            SysTrayIcon.Text = pTrayText;
            SysTrayIcon.Icon = pIcon;
        }

        public SystemTray(string pTrayText,Icon pIcon,bool pAnimate)
        {
            InitDefaults();
            SysTrayIcon.Text = pTrayText;
            SysTrayIcon.Icon = pIcon;
            bAnimate = pAnimate;
        }

        private void InitDefaults()
        {
            SysTrayIcon = new NotifyIcon();
            _mAnimTimer = new Timer();
            _mAnimTimer.Interval = 32;
            //Set Handler
            _mAnimTimer.Tick += SysTrayAnimator;
            SysTrayIcon.Visible = true;
            bAnimate = false;
        }

        public void Dispose()
        {
            SysTrayIcon.Dispose();
            _mAnimTimer.Dispose();
        }


        public Icon Icon
        {
            get => SysTrayIcon.Icon;
            set => SysTrayIcon.Icon = value;
        }
 
        public bool Visible
        {
            get => SysTrayIcon.Visible;
            set => SysTrayIcon.Visible = value;
        }
        public string TrayText
        {
            get => SysTrayIcon.Text;
            set => SysTrayIcon.Text = value;
        }

        public bool Animate
        {
            get => bAnimate;
            set => bAnimate = value;
        }

        public string BaloonTipText
        {
            get => SysTrayIcon.BalloonTipText;
            set => SysTrayIcon.BalloonTipText = value;
        }

        public string BalloonTipTitle
        {
            get => SysTrayIcon.BalloonTipTitle;
            set => SysTrayIcon.BalloonTipTitle = value;
        }

        public ToolTipIcon BalloonTipIcon
        {
            get => SysTrayIcon.BalloonTipIcon;
            set => SysTrayIcon.BalloonTipIcon = value;
        }

        public void SetContextMenuOrStrip(object value)
        {
                if (value.GetType().Name == "ContextMenu")
                {
                    SysTrayIcon.ContextMenu = (ContextMenu)value;
                }

                if (value.GetType().Name == "ContextMenuStrip")
                {
                    SysTrayIcon.ContextMenuStrip = (ContextMenuStrip)value;
                }
        }
        
        public ContextMenu ContextMenu
        {
            get => SysTrayIcon.ContextMenu;
            set => SysTrayIcon.ContextMenu = value;
        }
        
        public ContextMenuStrip ContextMenuStrip
        {
            get => SysTrayIcon.ContextMenuStrip;
            set => SysTrayIcon.ContextMenuStrip = value;
        }  
        public void StartAnimation()
        {
            if(bAnimate)
                _mAnimTimer.Start();
        }
        public void StopAnimation()
        {
            if (bAnimate)
                _mAnimTimer.Stop();
        }

        public void StartAnimation(int timeOut)
        {
            if (bAnimate)
            {
                _bIsTimeOut = true;
                _mAnimTimer.Start();
            }
        }

        public void ShowBaloonTrayTip(int nTimeout)
        {
            SysTrayIcon.ShowBalloonTip(nTimeout);
        }
        public void ShowBaloonTrayTip(int nTimeOut,string tipTitle,string tipText,ToolTipIcon tipIcon)
        {
            SysTrayIcon.ShowBalloonTip(nTimeOut, tipTitle, tipText, tipIcon);
        }

        private void SetIconRange(object[] IconList)
        {
            var tp = IconList[0].GetType();
            if(tp.Name == "String")
            {
                _animIconList = new Icon[IconList.Length];
                for (var i = 0; i < IconList.Length; ++i)
                {
                    _animIconList[i] = new Icon(IconList[i].ToString());
                }
                
            }
            if (tp.Name == "Icon")
            {
                _animIconList = new Icon[IconList.Length];
                for (var i = 0; i < IconList.Length; ++i)
                {
                    _animIconList[i] = (Icon)IconList[i];
                }
            }
            if (IconList.Length > 0)
                Icon = _animIconList[0];
        }

        public void SetIconSets(Dictionary<ServiceState, string> iconSets, ServiceState defaultSet = ServiceState.Normal)
        {
            _iconSets = iconSets;
            bAnimate = true;
            SetActiveSet(defaultSet);
        }

        public void SetActiveSet(ServiceState setKey)
        {
            var assembly = Assembly.GetExecutingAssembly();
        
            var icons = assembly.GetManifestResourceNames()
                .Where(entry => entry.Contains($"TrayIcons.{_iconSets[setKey]}"));
            _animIconList = icons?.Select(ir =>
                {
                    try
                    {
                        return new Bitmap(assembly.GetManifestResourceStream(ir));
                    }
                    catch (Exception e)
                    {
                        return new Bitmap(150, 150);
                    }
                })
                .Select(bp => Icon.FromHandle(bp.GetHicon())).ToArray();
            if (_animIconList?.Length < _animationIndex)
            {
                _animationIndex = 0;
            }

            Icon = _animIconList[_animationIndex] ?? Icon.ExtractAssociatedIcon(assembly.Location);
        }

        private void SysTrayAnimator(object sender, EventArgs e)
        {
            if(_animationIndex >= _animIconList.Length)
            {
                _animationIndex = 0;
            }
            SysTrayIcon.Icon = _animIconList[_animationIndex++];
        }

        /////////////////////////////////////////////////////////////////////////////
        /// <summary>
        /// ///////// Event handlers 
        /// </summary>

        void SysTrayIcon_MouseMove(object sender, MouseEventArgs e)
        {
            if(MouseMove !=null)
                MouseMove(sender, e);
        }

        void SysTrayIcon_MouseDown(object sender, MouseEventArgs e)
        {
            if (MouseDown != null)
                MouseDown(sender, e);
        }

        void SysTrayIcon_MouseDoubleClick(object sender, MouseEventArgs e)
        {
            if (MouseDoubleClick != null)
                MouseDoubleClick(sender, e);
        }

        void SysTrayIcon_MouseClick(object sender, MouseEventArgs e)
        {
            if (MouseClick != null)
                MouseClick(sender, e);
        }

        void SysTrayIcon_Click(object sender, EventArgs e)
        {
            if (Click != null)
                Click(sender, e);
        }

        void SysTrayIcon_MouseUp(object sender, MouseEventArgs e)
        {
            if (MouseUp != null)
                MouseUp(sender, e);
        }

        void SysTrayIcon_DoubleClick(object sender, EventArgs e)
        {
            if (DoubleClick != null)
                DoubleClick(sender, e);
        }

        void SysTrayIcon_BalloonTipShown(object sender, EventArgs e)
        {
            if (BalloonTipShown != null)
                BalloonTipShown(sender, e);
        }

        void SysTrayIcon_BalloonTipClosed(object sender, EventArgs e)
        {
            if (BalloonTipClosed != null)
                BalloonTipClosed(sender, e);
        }

        void SysTrayIcon_BalloonTipClicked(object sender, EventArgs e)
        {
            if (BalloonTipClicked != null)
                BalloonTipClicked(sender,e);
        }
    }
}
