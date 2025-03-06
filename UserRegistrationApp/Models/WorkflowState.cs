using UserRegistrationApp.DbContext;

namespace UserRegistrationApp.Models
{
    /// <summary>
    /// Represents a workflow state in the application.
    /// </summary>
    public class WorkflowState
    {
        /// <summary>
        /// Gets or sets the workflow state ID.
        /// </summary>
        public int WorkflowStateId { get; set; }

        /// <summary>
        /// Gets or sets the user ID associated with the workflow state.
        /// </summary>
        public int UserId { get; set; }

        /// <summary>
        /// Gets or sets the state of the workflow.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        /// Gets or sets the date the workflow state was created.
        /// </summary>
        public DateTime CreatedDate { get; set; }

        /// <summary>
        /// Gets or sets the user associated with the workflow state.
        /// </summary>
        public User User { get; set; }
    }
}


