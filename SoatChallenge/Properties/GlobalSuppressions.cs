[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdsShouldBeSpelledCorrectly", MessageId = "Soat",
                              Justification = "Soat is the vendor name")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdsShouldBeSpelledCorrectly", MessageId = "Soat", Scope = "namespace", Target = "SoatChallenge",
                              Justification = "Soat is the vendor name")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames",
                              Justification = "PFX signed file will be assigned on release")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Scope = "member", Target = "SoatChallenge.Path.#FilterAvailableDirections(System.Collections.Generic.IEnumerable`1<SoatChallenge.RouteCell>,SoatChallenge.Route+Specs)",
                              Justification = "Linq queries rise complexity, method has already been refactored")]