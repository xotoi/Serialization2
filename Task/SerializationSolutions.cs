using Microsoft.VisualStudio.TestTools.UnitTesting;
using Task.DB;
using System.Runtime.Serialization;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using Task.Surrogates;
using Task.TestHelpers;

namespace Task
{
	[TestClass]
	public class SerializationSolutions
	{
	    private Northwind _dbContext;

		[TestInitialize]
		public void Initialize()
		{
			_dbContext = new Northwind();
		}

        [TestMethod]
        public void SerializationCallbacks()
        {
            _dbContext.Configuration.ProxyCreationEnabled = false;

            var serializationContext = new SerializationContext
            {
                ObjectContext = (_dbContext as IObjectContextAdapter).ObjectContext,
                TypeToSerialize = typeof(Category)
            };
            var xmlSerializer = new NetDataContractSerializer(new StreamingContext(StreamingContextStates.All, serializationContext));

            var serializer = new XmlDataContractSerializer<IEnumerable<Category>>(xmlSerializer);
            var tester = new SerializationTester<IEnumerable<Category>>(serializer, true);
            var categories = _dbContext.Categories.ToList();

            tester.SerializeAndDeserialize(categories);
        }

        [TestMethod]
        public void ISerializable()
        {
            _dbContext.Configuration.ProxyCreationEnabled = false;

            var serializationContext = new SerializationContext
            {
                ObjectContext = (_dbContext as IObjectContextAdapter).ObjectContext,
                TypeToSerialize = typeof(Product)
            };
            var xmlSerializer = new NetDataContractSerializer(new StreamingContext(StreamingContextStates.All, serializationContext));
            var serializer = new XmlDataContractSerializer<IEnumerable<Product>>(xmlSerializer);
            var tester = new SerializationTester<IEnumerable<Product>>(serializer, true);
            var products = _dbContext.Products.ToList();

            tester.SerializeAndDeserialize(products);
        }

        [TestMethod]
        public void ISerializationSurrogate()
        {
            _dbContext.Configuration.ProxyCreationEnabled = false;

            var serializationContext = new SerializationContext
            {
                ObjectContext = (_dbContext as IObjectContextAdapter).ObjectContext,
                TypeToSerialize = typeof(Order_Detail)
            };

            var xmlSerializer = new NetDataContractSerializer()
            {
                SurrogateSelector = new OrderDetailSurrogateSelector(),
                Context = new StreamingContext(StreamingContextStates.All, serializationContext)
            };
            var serializer = new XmlDataContractSerializer<IEnumerable<Order_Detail>>(xmlSerializer);
            var tester = new SerializationTester<IEnumerable<Order_Detail>>(serializer, true);
            var orderDetails = _dbContext.Order_Details.ToList();

            tester.SerializeAndDeserialize(orderDetails);
        }

        [TestMethod]
        public void IDataContractSurrogate()
        {
            _dbContext.Configuration.ProxyCreationEnabled = true;
            _dbContext.Configuration.LazyLoadingEnabled = true;

            var xmlSerializer = new DataContractSerializer(typeof(IEnumerable<Order>),
                new DataContractSerializerSettings
                {
                    DataContractSurrogate = new OrderDataContractSurrogate()
                });
            var serializer = new XmlDataContractSerializer<IEnumerable<Order>>(xmlSerializer);
            var tester = new SerializationTester<IEnumerable<Order>>(serializer, true);
            var orders = _dbContext.Orders.ToList();

            tester.SerializeAndDeserialize(orders);
        }
    }
}
