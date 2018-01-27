using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public interface IReceiver
{
    void Receive(Transmission t);
    void Recall(Transmission t);
}
