namespace KingHostManager
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
            this.KingHostManagerProcessInstaller = new System.ServiceProcess.ServiceProcessInstaller();
            this.KingHostManagerInstaller = new System.ServiceProcess.ServiceInstaller();
            // 
            // KingHostManagerProcessInstaller
            // 
            this.KingHostManagerProcessInstaller.Account = System.ServiceProcess.ServiceAccount.LocalSystem;
            this.KingHostManagerProcessInstaller.Password = null;
            this.KingHostManagerProcessInstaller.Username = null;
            // 
            // KingHostManagerInstaller
            // 
            this.KingHostManagerInstaller.Description = "KingHostManager";
            this.KingHostManagerInstaller.DisplayName = "KingHostManager";
            this.KingHostManagerInstaller.ServiceName = "KingHostManagerService";
            this.KingHostManagerInstaller.StartType = System.ServiceProcess.ServiceStartMode.Automatic;
            // 
            // ProjectInstaller
            // 
            this.Installers.AddRange(new System.Configuration.Install.Installer[] {
            this.KingHostManagerInstaller,
            this.KingHostManagerProcessInstaller});

        }

        #endregion

        private System.ServiceProcess.ServiceProcessInstaller KingHostManagerProcessInstaller;
        private System.ServiceProcess.ServiceInstaller KingHostManagerInstaller;
    }
}