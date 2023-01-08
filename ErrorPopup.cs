using Godot;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public partial class ErrorPopup : Node
{
    private List<ErrorTracker> m_ErrorPanes = new List<ErrorTracker>();

    /// <summary>
    /// Max number of errors we will show.
    /// </summary>
    private const int MAX_ERRORS = 5;

    /// <summary>
    /// Class which tracks the Node2D of the ErrorPane as well as the time it was created
    /// </summary>
    private class ErrorTracker
    {
        public Control ErrorPane { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    /// <summary>
    /// The scene prefab set in editor for the error pane.
    /// </summary>
    [Export]
    public PackedScene ErrorPaneScene;
    
    /// <summary>
    /// The node which contains the error panels.
    /// </summary>
    [Export]
    public Container ErrorContainer;
    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        ShowError("This is a test error");
    }

    private double timer = 0.0;
    private int counter = 1;

    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _Process(double delta)
    {
        timer += delta;
        if (timer > .9)
        {
            timer = 0;
            counter++;
        }
        
        // Calls CleanUpError on each expired error
        foreach (var error in m_ErrorPanes)
        {
            if (error.ExpiresAt < DateTime.Now)
            {
                CleanupError(error);
            }
        }
    }

    /// <summary>
    /// Function which instantiates a new error pane and adds it to the list of panes.
    /// Each pane should only last on the screen for 5 seconds before fading.
    /// </summary>
    /// <param name="error"></param>
    public void ShowError(string error)
    {
        var errorPane = ErrorPaneScene.Instantiate() as Control;
        errorPane.GetNode<Label>("Label").Text = error;
        ErrorContainer.AddChild(errorPane);

        var errorTracker = new ErrorTracker
        {
            ErrorPane = errorPane,
            ExpiresAt = DateTime.Now.AddSeconds(5)
        };
        m_ErrorPanes.Add(errorTracker);
        
        if (m_ErrorPanes.Count > MAX_ERRORS)
        {
            CleanupError(m_ErrorPanes[0]);
        }
    }

    /// <summary>
    /// Removes the errorPane child and removes the errorTracker from the list of panes.
    /// </summary>
    /// <param name="errorTracker"></param>
    private void CleanupError(ErrorTracker errorTracker)
    {
        ErrorContainer.RemoveChild(errorTracker.ErrorPane);
        m_ErrorPanes.Remove(errorTracker);
    }
}
