using Lucene.Net.Analysis.Standard;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using Lucene.Net.Util;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Toxon.Micro.Blog.Search.Inbound;

namespace Toxon.Micro.Blog.Search
{
    public class BusinessLogic : HostPlugin
    {
        private IndexWriter _writer;

        public override void Init(Host host)
        {
            var luceneVersion = LuceneVersion.LUCENE_48;

            var analyzer = new StandardAnalyzer(luceneVersion);
            var dir = new RAMDirectory();

            var indexConfig = new IndexWriterConfig(luceneVersion, analyzer);
            _writer = new IndexWriter(dir, indexConfig);


            host.Add<SearchInsertRequest>("search:insert", HandleInsert);
            host.Add<SearchQueryRequest>("search:query", HandleQuery);
        }

        private Task<object> HandleInsert(SearchInsertRequest message)
        {
            var document = new Lucene.Net.Documents.Document
            {
                new StringField("kind", message.Kind, Field.Store.YES),
                new Int32Field("id", message.Id, Field.Store.YES)
            };

            foreach (var field in message.Fields)
            {
                document.Add(new TextField($"f_{field.Key.ToLower()}", field.Value, Field.Store.YES));
            }

            _writer.AddDocument(document);
            _writer.Flush(triggerMerge: false, applyAllDeletes: false);

            return Task.FromResult<object>(null);
        }

        private Task<object> HandleQuery(SearchQueryRequest message)
        {
            var query = new BooleanQuery()
            {
                new BooleanClause(new TermQuery(new Term("kind", message.Kind)), Occur.MUST),
                new BooleanClause(new MultiPhraseQuery
                {
                    // TODO generalise
                    new Term("f_text", message.Query)
                }, Occur.SHOULD),
            };


            var searcher = new IndexSearcher(_writer.GetReader(applyAllDeletes: true));
            var hits = searcher.Search(query, 20);

            var results = new List<SearchResult>();
            foreach (var hit in hits.ScoreDocs)
            {
                var foundDoc = searcher.Doc(hit.Doc);

                var fields = new Dictionary<string, string>();
                foreach (var field in foundDoc.Fields)
                {
                    if (!field.Name.StartsWith("f_"))
                    {
                        continue;
                    }

                    fields.Add(field.Name.Substring(2), field.GetStringValue());
                }

                results.Add(new SearchResult
                {
                    Score = (decimal)hit.Score,
                    Document = new Inbound.Document
                    {
                        Kind = foundDoc.Get("kind"),
                        Id = foundDoc.GetField("id").GetInt32ValueOrDefault(),
                        Fields = fields,
                    }
                });
            }

            return Task.FromResult<object>(new SearchQueryResponse { Results = results });
        }
    }
}
