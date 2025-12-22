namespace ScopesTest
{
    using System.Collections.Generic;
    using System.Linq;

    public static class ScopeParser
    {
        /*
         * 1)
         * If the request scope contains scope '.default'
         * we return all the scopes in the registration.
         * 
         * 2)
         * If the request has a scope that is not
         * in the registration an error is returned.
         * 
         * 3)
         * If the request contains scope 'xyz.default'
         * we return all the scopes in the registration starting with 'xyz.'.
         * (if the registration does not contain any 'xyz.' scopes an error is returned)
         * 
         * 4)
         * All scopes in the request that do not end with '.default'
         * are returned (if they exist in the registration, @see 2).
         */

        public static string GetScopes(string registrationScopes,
            string requestScopes, out string error)
        {
            if (string.IsNullOrWhiteSpace(requestScopes))
            {
                error = "There are no request scopes.";
                return null;
            }

            string[] regScopes = $"{registrationScopes}".Split(' ');
            string[] reqScopes = requestScopes.Split(' ');
            error = null;

            var scopes = new HashSet<string>();

            foreach (string reqScope in reqScopes)
            {
                if (reqScope == ".default")
                {
                    AddRange(scopes, regScopes);
                    break;
                }

                if (reqScope.EndsWith(".default"))
                {
                    int index = reqScope.LastIndexOf('.');
                    string reqPrefix = reqScope.Substring(0, index);

                    IEnumerable<string> prefixScopes =
                        GetScopesByPrefix(reqPrefix, regScopes);

                    if (!prefixScopes.Any())
                    {
                        error =
                            $"Registration does not have any '{reqPrefix}.*' scopes.";
                        return null;
                    }

                    AddRange(scopes, prefixScopes);

                    continue;
                }

                if (!regScopes.Contains(reqScope))
                {
                    error = $"Registration does not contain scope '{reqScope}'.";
                    return null;
                }

                scopes.Add(reqScope);
            }

            string result = string.Join(' ', scopes);

            return result;
        }

        private static void AddRange(
            HashSet<string> hashSet, IEnumerable<string> range)
        {
            foreach (string rng in range)
            {
                hashSet.Add(rng);
            }
        }

        private static IEnumerable<string> GetScopesByPrefix(
            string prefix, IEnumerable<string> scopes)
        {
            var result = new List<string>();

            foreach (string scope in scopes)
            {
                int index = scope.LastIndexOf('.');

                if (index < 0)
                {
                    continue;
                }

                string ratgetPrefix = scope.Substring(0, index);

                if (ratgetPrefix == prefix)
                {
                    result.Add(scope);
                }
            }

            return result;
        }
    }
}
