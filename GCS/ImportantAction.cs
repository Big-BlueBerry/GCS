using System.Linq;
using System.Collections.Generic;
using Grid.Framework;
using Grid.Framework.Components;
using GCS.Rules;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;


namespace GCS
{
    public enum userActions
    {
        CREATE,
        DELETE,
        MOVE
    }

    public class ImportantAction
    {
        public userActions action;
        public Shape subject;
        public Vector2 [] movInfo;
        
        public ImportantAction(userActions action, Shape subject, Vector2 [] movInfo )
        {
            this.action = action;
            this.subject = subject;
            this.movInfo = movInfo;
        }

    }
 
}
