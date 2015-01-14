namespace ScenarioTools.Reporting
{
    public class DocumentHolder
    {
        #region Constructors
        public DocumentHolder()
        {
            SADocument = null;
        }
        public DocumentHolder(SADocument document)
        {
            SADocument = document;
        }
        #endregion Constructors

        #region Properties
        public SADocument SADocument { get; set; }
        #endregion Properties
    }
}
