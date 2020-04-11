namespace MultimediaIOManagerService
{
    partial class ProjectInstaller
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.MIOMServiceProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.MultimediaIOManagerServiceInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // MIOMServiceProcessInstaller
            // 
            this.MIOMServiceProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.MIOMServiceProcessInstaller.Password = null;
            this.MIOMServiceProcessInstaller.Username = null;
            // 
            // MultimediaIOManagerServiceInstaller
            // 
            this.MultimediaIOManagerServiceInstaller.Description = "Multimedia IO Manager Service";
            this.MultimediaIOManagerServiceInstaller.DisplayName = "MIOMS";
            this.MultimediaIOManagerServiceInstaller.ServiceName = "MultimediaIOManagerService";
            this.MultimediaIOManagerServiceInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.MIOMServiceProcessInstaller,
            this.MultimediaIOManagerServiceInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller MIOMServiceProcessInstaller;
        private System.ServiceProcess.ServiceInstaller MultimediaIOManagerServiceInstaller;
    }
}