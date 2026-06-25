namespace EduProgress.Pages;

partial class CourseEditorPage
{
    private System.ComponentModel.IContainer? components = null;
    protected override void Dispose(bool disposing)
    { if (disposing && components != null) components.Dispose(); base.Dispose(disposing); }

    private void InitializeComponent()
    {
        this.SuspendLayout();
        this.Dock      = DockStyle.Fill;
        this.BackColor = AppTheme.Background;
        this.ResumeLayout(false);
    }
}
