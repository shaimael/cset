﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BusinessLogic.Helpers;
using CSETWeb_Api.BusinessManagers;
using CSETWeb_Api.BusinessManagers.Diagram.Analysis;

namespace CSETWeb_Api.BusinessLogic.BusinessManagers.Diagram.analysis.rules
{
    class Rule5 : AbstractRule, IRuleEvaluate
    {
        private String rule5 = "The path identified by the components, {0} and {1}, appears to connect on one side to an external network.  A firewall to filter the traffic to and from the external network is recommended to protect the facility's network.  Note that a 'Web' component, 'Vendor' component, or 'Partner' component are all assumed to interface with an external network.  In addition, a modem with a single connection is assumed to allow a connection from outside the facility's network.";
        private SimplifiedNetwork network;

        public Rule5(SimplifiedNetwork simplifiedNetwork)
        {
            this.network = simplifiedNetwork;
        }

        public List<IDiagramAnalysisNodeMessage> Evaluate()
        {
            var partenerVendorWeb = network.Nodes.Values.Where(x => x.IsPartnerVendorOrWeb).ToList();
            foreach(var pvw in partenerVendorWeb)
            {
                CheckRule5(pvw);
            }
            return this.Messages;
        }


        private void CheckRule5(NetworkComponent component)
        {
            if (component.IsPartnerVendorOrWeb)//Is it Firewall,Vendor,Partner
            {
                HashSet<String> VisitedNodes = new HashSet<string>();
                VisitedNodes.Add(component.ID);
                List<NetworkComponent> list = GetNodeEdges(component, new HashSet<int>() { Constants.FIREWALL });
                foreach (NetworkComponent info in list)
                {
                    String componentName = "unnamed";
                    if (!String.IsNullOrWhiteSpace(component.ComponentName))
                    {
                        componentName = component.ComponentName;
                    }

                    String endComponentName = "unnamed";
                    if (!String.IsNullOrWhiteSpace(info.ComponentName))
                    {
                        endComponentName = info.ComponentName;
                    }

                    String text = String.Format(rule5, componentName, endComponentName);
                    SetLineMessage(component, info, text);
                }
            }
        }

    }
}
