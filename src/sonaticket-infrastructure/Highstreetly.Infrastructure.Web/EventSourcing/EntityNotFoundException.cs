using System;

namespace Highstreetly.Infrastructure.EventSourcing
{
    public class EntityNotFoundException : Exception
    {
        private readonly Guid entityId;
        private readonly string entityType;

        public EntityNotFoundException()
        {
        }

        public EntityNotFoundException(Guid entityId) : base(entityId.ToString())
        {
            this.entityId = entityId;
        }

        public EntityNotFoundException(Guid entityId, string entityType)
            : base(entityType + ": " + entityId.ToString())
        {
            this.entityId = entityId;
            this.entityType = entityType;
        }

        public EntityNotFoundException(Guid entityId, string entityType, string message, Exception inner)
            : base(message, inner)
        {
            this.entityId = entityId;
            this.entityType = entityType;
        }

        //        protected EntityNotFoundException(
        //            SerializationInfo info,
        //            StreamingContext context) : base(info, context)
        //        {
        //            if (info == null)
        //                throw new ArgumentNullException("info");
        //
        //            this.entityId = Guid.Parse(info.GetString("entityId"));
        //            this.entityType = info.GetString("entityType");
        //        }

        public Guid EntityId
        {
            get { return entityId; }
        }

        public string EntityType
        {
            get { return entityType; }
        }


        //        [SecurityPermission(SecurityAction.Demand, SerializationFormatter = true)]
        //        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        //        {
        //            base.GetObjectData(info, context);
        //            info.AddValue("entityId", this.entityId.ToString());
        //            info.AddValue("entityType", this.entityType);
        //        }
    }
}