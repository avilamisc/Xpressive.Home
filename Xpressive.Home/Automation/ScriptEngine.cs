﻿using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xpressive.Home.Contracts.Automation;

namespace Xpressive.Home.Automation
{
    internal class ScriptEngine : IScriptEngine
    {
        private readonly IList<IScriptObjectProvider> _scriptObjectProviders;
        private readonly IScriptRepository _scriptRepository;

        public ScriptEngine(IEnumerable<IScriptObjectProvider> scriptObjectProviders, IScriptRepository scriptRepository)
        {
            _scriptRepository = scriptRepository;
            _scriptObjectProviders = scriptObjectProviders.ToList();
        }

        public async Task ExecuteAsync(string scriptId)
        {
            var script = await _scriptRepository.GetAsync(scriptId);
            Execute(script);
        }

        private void Execute(Script script)
        {
            if (script == null)
            {
                return;
            }

            var context = new ScriptExecutionContext(script, _scriptObjectProviders);
            context.Execute();
        }
    }
}
