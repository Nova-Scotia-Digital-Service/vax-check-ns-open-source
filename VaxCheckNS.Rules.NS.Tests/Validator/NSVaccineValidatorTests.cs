using System;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using VaxCheckNS.Rules.NS.Models;
using VaxCheckNS.Rules.NS.Validator;
using Xunit;

namespace VaxCheckNS.Rules.NS.Tests.Validator
{
    public class NSVaccineValidatorTests
    {
        private const string fhirBundleWithTwoValidDoubleDosageWithMinimumWait =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                },
                {
                   'fullUrl':'resource:2',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-28',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]
            }}";

        private const string fhirBundleWithOneValidSingleDosageWithMinimumWait =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'212'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private const string fhirBundleWithoutTwoValidDoubleDosageWithMinimumWait =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private const string fhirBundleWithSingleInvalidSingeDosageWithMinimumWait =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'500'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private const string fhirBundleWithTwoInvalidDoubleDosageWithMinimumWait =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'501'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                },
                {
                   'fullUrl':'resource:2',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'501'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-28',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private const string fhirBundleWithOneInvalidCodeDosageWithTwoValidDosageWithMinimumWait =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'501'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                },
                {
                   'fullUrl':'resource:2',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'207'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-28',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                },
                {
                   'fullUrl':'resource:3',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-28',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private string fhirBundleWithTwoValidDoubleDosageWithoutMinimumWait =
            @$"{{'fhirBundle':{{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {{
                   'fullUrl':'resource:0',
                   'resource':{{
                      'resourceType':'Patient',
                      'name':[
                         {{
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }}
                      ],
                      'birthDate':'1942-04-10'
                   }}
                }},
                {{
                   'fullUrl':'resource:1',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }},
                {{
                   'fullUrl':'resource:2',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }}
             ]}}}}";

        private string fhirBundleWithOneValidSingleDosageWithoutMinimumWait =
            @$"{{'fhirBundle':{{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {{
                   'fullUrl':'resource:0',
                   'resource':{{
                      'resourceType':'Patient',
                      'name':[
                         {{
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }}
                      ],
                      'birthDate':'1942-04-10'
                   }}
                }},
                {{
                   'fullUrl':'resource:2',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }}
             ]}}}}";

        private string fhirBundleWithValidDoubleDosageWithoutMinimumWaitOnBoosterShot =
        @$"{{'fhirBundle':{{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {{
                   'fullUrl':'resource:0',
                   'resource':{{
                      'resourceType':'Patient',
                      'name':[
                         {{
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }}
                      ],
                      'birthDate':'1942-04-10'
                   }}
                }},
                {{
                   'fullUrl':'resource:2',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(50)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }},
                {{
                   'fullUrl':'resource:1',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(30)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }},
                {{
                   'fullUrl':'resource:2',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(1)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }}
             ]}}}}";

        private string fhirBundleWithValidSingleDosageWithoutMinimumWaitOnBoosterShot =
        @$"{{'fhirBundle':{{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {{
                   'fullUrl':'resource:0',
                   'resource':{{
                      'resourceType':'Patient',
                      'name':[
                         {{
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }}
                      ],
                      'birthDate':'1942-04-10'
                   }}
                }},
                {{
                   'fullUrl':'resource:1',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'212'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(50)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }},
                {{
                   'fullUrl':'resource:2',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(30)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }}
             ]}}}}";

        private string fhirBundleWithValidDosageWithExactMinimumWait =
            @$"{{'fhirBundle':{{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {{
                   'fullUrl':'resource:0',
                   'resource':{{
                      'resourceType':'Patient',
                      'name':[
                         {{
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }}
                      ],
                      'birthDate':'1942-04-10'
                   }}
                }},
                {{
                   'fullUrl':'resource:2',
                   'resource':{{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{{
                         'coding':[
                            {{
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'212'
                            }}
                         ]
                      }},
                      'patient':{{
                         'reference':'resource:0'
                      }},
                      'occurrenceDateTime':'{DateTimeOffset.Now.Subtract(TimeSpan.FromDays(14)):yyyy-MM-dd}',
                      'performer':[
                         {{
                            'actor':{{
                               'display':'Nova Scotia, Canada'
                            }}
                         }}
                      ],
                      'lotNumber':'EP6775',
                      'manufacturer':{{
                         'identifier':{{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }}
                      }}
                   }}
                }}
             ]}}}}";

        private const string fhirBundleWithoutTwoValidDoubleDosageWithBotchedOccuranceDate =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'208'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'20S21-0233-00923',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private const string fhirBundleWithMultipleVaccineSystemCodings =
        @"{'fhirBundle':{
             'resourceType':'Bundle',
             'type':'collection',
             'entry':[
                {
                   'fullUrl':'resource:0',
                   'resource':{
                      'resourceType':'Patient',
                      'name':[
                         {
                            'family':'Smith',
                            'given':[
                               'Jon'
                            ]
                         }
                      ],
                      'birthDate':'1942-04-10'
                   }
                },
                {
                   'fullUrl':'resource:1',
                   'resource':{
                      'isSubpotent':true,
                      'resourceType':'Immunization',
                      'status':'completed',
                      'vaccineCode':{
                         'coding':[
                            {
                               'system':'http://hl7.org/fhir/sid/cvx',
                               'code':'212'
                            },
                            {
                                'system':'http://snomed.info/sct',
                                'code':'2896100087105'
                            }
                         ]
                      },
                      'patient':{
                         'reference':'resource:0'
                      },
                      'occurrenceDateTime':'2021-03-03',
                      'performer':[
                         {
                            'actor':{
                               'display':'Nova Scotia, Canada'
                            }
                         }
                      ],
                      'lotNumber':'EL1404',
                      'manufacturer':{
                         'identifier':{
                            'system':'http://hl7.org/fhir/sid/mvx',
                            'value':'PFR'
                         }
                      }
                   }
                }
             ]}}";

        private readonly IList<ValidVaccine> _vaccineList = new List<ValidVaccine>
        {
            new ValidVaccine("Moderna", "207", 2),
            new ValidVaccine("Pfizer", "208", 2),
            new ValidVaccine("AstraZenica", "210", 2),
            new ValidVaccine("J&J", "212", 1),
        };

        private JObject _fhirBundleObj;

        public NSVaccineValidatorTests()
        {
            // Default.
            _fhirBundleObj = JObject.Parse(fhirBundleWithTwoValidDoubleDosageWithMinimumWait);
        }

        [Fact]
        public void CanInstantiate()
        {
            // Arrange, Act, Assert
            Assert.NotNull(new NSVaccineValidator(_fhirBundleObj, _vaccineList));
        }

        [Fact]
        public void Validate_GivenTwoValidDoseAdministeredAndAtLeastMinimumDaysVaccinated_ResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithTwoValidDoubleDosageWithMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Validate_GivenTwoValidDoseAdministeredButMinimumDaysVaccinatedNotMet_ResultIsFailure()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithTwoValidDoubleDosageWithoutMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Failure && result.Message == VaccineStatus.InvalidOccuranceDate);
        }

        [Fact]
        public void Validate_GivenTwoValidDoseAdministeredButExactlyMinimumDaysVaccinatedMet_ResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithValidDosageWithExactMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Validate_GivenOnlyOneValidDoseAdministeredForNonSingleDoseAndAtLeastMinimumDaysVaccinated_ResultIsFailure()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithoutTwoValidDoubleDosageWithMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Failure && result.Message == VaccineStatus.InvalidDosageCount);
        }

        [Fact]
        public void Validate_GivenOneValidDoseAdministeredForSingleDoseAndAtLeastMinimumDaysVaccinated_ResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithOneValidSingleDosageWithMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Validate_GivenOneValidDoseAdministeredForSingleDoseButMinimumDaysVaccinatedNotMet_ResultIsFailure()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithOneValidSingleDosageWithoutMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Failure && result.Message == VaccineStatus.InvalidOccuranceDate);
        }

        [Fact]
        public void Validate_GivenTwoInvalidDoseAdministeredAndAtLeastMinimumDaysVaccinated_ResultIsFailure()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithTwoInvalidDoubleDosageWithMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Failure && result.Message == VaccineStatus.InvalidVaccineCode);
        }

        [Fact]
        public void Validate_GivenOneInvalidSingleDoseAdministeredAndAtLeastMinimumDaysVaccinated_ResultIsFailure()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithSingleInvalidSingeDosageWithMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Failure && result.Message == VaccineStatus.InvalidVaccineCode);
        }

        [Fact]
        public void Validate_GivenBotchedOccurenceDate_ResultIsFailure()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithoutTwoValidDoubleDosageWithBotchedOccuranceDate);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Failure && result.Message == VaccineStatus.InvalidFormat);
        }

        [Fact]
        public void Validate_GivenMultipleVaccineCodingTypes_CvxIsUsedAndResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithMultipleVaccineSystemCodings);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Validate_GivenOneUnknownDoseAndTwoValidDosagesAdministeredAndAtLeastMinimumDaysVaccinated_ResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithOneInvalidCodeDosageWithTwoValidDosageWithMinimumWait);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Validate_GivenValidDoubleDoseAndBoosterShotAdministeredUnder14DaysAgo_ResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithValidDoubleDosageWithoutMinimumWaitOnBoosterShot);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }

        [Fact]
        public void Validate_GivenValidSingleDoseAndBoosterShotAdministeredUnder14DaysAgo_ResultIsSuccess()
        {
            // Arrange,
            _fhirBundleObj = JObject.Parse(fhirBundleWithValidSingleDosageWithoutMinimumWaitOnBoosterShot);
            var vaccineValidator = new NSVaccineValidator(_fhirBundleObj, _vaccineList);
            // Act,
            var result = vaccineValidator.Validate();
            // Assert
            Assert.True(result.Success);
        }
    }
}
