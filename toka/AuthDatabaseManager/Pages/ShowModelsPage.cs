namespace AuthDatabaseManager.Pages
{
    using AuthDatabaseManager.Models;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public abstract class ShowModelsPage<T> : ModelPage<T> where T : Model, new()
    {
        protected Func<IEnumerable<T>> GetRecords { get; init; }

        protected IEnumerable<T> records;


        public ShowModelsPage(Page returnPage,
            Database database, Func<IEnumerable<T>> getRecords = null)
            : base(returnPage, database)
        {
            this.GetRecords = getRecords;
        }


        public override Page Show()
        {
            IEnumerable<T> preShow = this.GetRecords.Invoke();

            T instance = Activator.CreateInstance<T>();

            this.records = instance.PreShow(preShow.ToArray());

            var grid = instance.GetGrid(this.database, this.records);

            var list = new List<Row>();

            T record = this.records.FirstOrDefault();

            if (record != null)
            {
                list.Add(new Row(record.Columns));
            }

            list.AddRange(grid);

            IEnumerable<Row> aligned = Helper.Align(list);

            List<Row> lines = aligned.ToList();

            if (!this.records.Any())
            {
                lines.Add(new Row("*there are no records to show"));
            }

            Write.Lines(lines);
            Console.WriteLine();

            return this.AfterShow();
        }

        protected abstract Page AfterShow();
    }
}
