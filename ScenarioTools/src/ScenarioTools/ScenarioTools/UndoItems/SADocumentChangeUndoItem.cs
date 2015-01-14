using ScenarioTools.Reporting;
using SoftwareProductions.Utilities.Undo;

namespace ScenarioTools.UndoItems
{
    public class SADocumentChangeUndoItem : UndoItemBase
    {
        #region Fields
        private SADocument _saDocumentOld;
        private SADocument _saDocumentNew;
        private DocumentHolder _documentHolder;
        #endregion Fields

        public SADocumentChangeUndoItem(DocumentHolder documentHolder, SADocument saDocumentOld, SADocument saDocumentNew)
        {
            _documentHolder = documentHolder;
            _saDocumentOld = new SADocument(saDocumentOld);
            _saDocumentNew = new SADocument(saDocumentNew);
        }

        #region Abstract methods of UndoItemBase
        public override void Undo()
        {
            // Restore original document to DocumentHolder
            _documentHolder.SADocument = _saDocumentOld;
        }
        public override void Redo()
        {
            // Restore new document to DocumentHolder
            _documentHolder.SADocument = _saDocumentNew;
        }
        #endregion Abstract methods of UndoItemBase
    }
}
