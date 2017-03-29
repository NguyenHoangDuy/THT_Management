using Kendo.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace THT.Service
{
    public class KendoApplyFilter
    {
        public string ApplyFilter(IFilterDescriptor filter)
        {
            return ApplyFilter(filter, "");
        }

        public string ApplyFilter(IFilterDescriptor filter, string id)
        {
            var filters = string.Empty;
            if (filter is CompositeFilterDescriptor)
            {
                filters += "(";
                var compositeFilterDescriptor = (CompositeFilterDescriptor)filter;
                foreach (IFilterDescriptor childFilter in compositeFilterDescriptor.FilterDescriptors)
                {
                    filters += ApplyFilter(childFilter, id);
                    filters += string.Format(" {0} ", compositeFilterDescriptor.LogicalOperator.ToString());
                }
            }
            else
            {
                string filterDescriptor = String.Empty;
                var descriptor = (FilterDescriptor)filter;
                string filterMember = descriptor.Member;
                string filterValue = descriptor.Value.ToString().Replace("'","''");
                if(descriptor.Operator.ToString() == "Contains")
                {
                    filterMember = filterMember + " COLLATE Latin1_General_CI_AI ";
                    filterValue = "'%" + THT.Models.CustomModel.ConvertToUnsign(filterValue) + "%'";

                }

                DateTime temp;

                switch (descriptor.Operator)
                {
                    case FilterOperator.IsEqualTo:
                        filterDescriptor += String.Format("{0} = N'{1}'", id + "[" + filterMember + "]", filterValue);                        
                        break;
                    case FilterOperator.IsNotEqualTo:
                        filterDescriptor += String.Format("{0} <> N'{1}'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.StartsWith:
                        filterDescriptor += String.Format("{0} like N'{1}%'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.Contains:
                        //filterDescriptor += String.Format("{0} like N'%{1}%'", id + "[" + filterMember + "]", filterValue);
                        filterDescriptor += String.Format("{0} like {1}", id + filterMember, filterValue);
                        break;
                    case FilterOperator.EndsWith:
                        filterDescriptor += String.Format("{0} like N'%{1}'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.IsLessThanOrEqualTo:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0} <='{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0} <={1}", id + "[" + filterMember + "]", filterValue);
                        }

                        break;
                    case FilterOperator.IsLessThan:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0}<'{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0}<{1}", id + "[" + filterMember + "]", filterValue);
                        }

                        break;
                    case FilterOperator.IsGreaterThanOrEqualTo:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0}>='{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0}>={1}", id + "[" + filterMember + "]", filterValue);
                        }

                        break;
                    case FilterOperator.IsGreaterThan:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0}>'{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0}>{1}", id + "[" + filterMember + "]", filterValue);
                        }

                        break;
                }

                filters = filterDescriptor;
            }

            filters = filters.EndsWith("And ") == true ? string.Format("{0})", filters.Substring(0, filters.Length - 4)) : filters;
            filters = filters.EndsWith("Or ") == true ? string.Format("{0})", filters.Substring(0, filters.Length - 4)) : filters;

            return filters;
        }

        public string ApplyFilterExclude(IFilterDescriptor filter, string id, List<String> exclude)
        {
            var filters = string.Empty;
            if (filter is CompositeFilterDescriptor)
            {
                filters += "(";
                var compositeFilterDescriptor = (CompositeFilterDescriptor)filter;
                foreach (IFilterDescriptor childFilter in compositeFilterDescriptor.FilterDescriptors)
                {
                    filters += ApplyFilterExclude(childFilter, id, exclude);
                    filters += string.Format(" {0} ", compositeFilterDescriptor.LogicalOperator.ToString());
                }
            }
            else
            {
                string filterDescriptor = String.Empty;
                var descriptor = (FilterDescriptor)filter;
                var filterMember = descriptor.Member;
                var filterValue = descriptor.Value;
                if (!exclude.Contains(descriptor.Member))
                {
                    DateTime temp;

                    switch (descriptor.Operator)
                    {
                        case FilterOperator.IsEqualTo:
                            filterDescriptor += String.Format("{0} = N'{1}'", id + "[" + filterMember + "]", filterValue);
                            break;
                        case FilterOperator.IsNotEqualTo:
                            filterDescriptor += String.Format("{0} <> N'{1}'", id + "[" + filterMember + "]", filterValue);
                            break;
                        case FilterOperator.StartsWith:
                            filterDescriptor += String.Format("{0} like N'{1}%'", id + "[" + filterMember + "]", filterValue);
                            break;
                        case FilterOperator.Contains:
                            filterDescriptor += String.Format("{0} like N'%{1}%'", id + "[" + filterMember + "]", filterValue);
                            break;
                        case FilterOperator.EndsWith:
                            filterDescriptor += String.Format("{0} like N'%{1}'", id + "[" + filterMember + "]", filterValue);
                            break;
                        case FilterOperator.IsLessThanOrEqualTo:
                            if (DateTime.TryParse(filterValue.ToString(), out temp))
                            {
                                filterDescriptor += String.Format("{0} <='{1}'", id + "[" + filterMember + "]", filterValue);
                            }
                            else
                            {
                                filterDescriptor += String.Format("{0} <={1}", id + "[" + filterMember + "]", filterValue);
                            }

                            break;
                        case FilterOperator.IsLessThan:
                            if (DateTime.TryParse(filterValue.ToString(), out temp))
                            {
                                filterDescriptor += String.Format("{0}<'{1}'", id + "[" + filterMember + "]", filterValue);
                            }
                            else
                            {
                                filterDescriptor += String.Format("{0}<{1}", id + "[" + filterMember + "]", filterValue);
                            }

                            break;
                        case FilterOperator.IsGreaterThanOrEqualTo:
                            if (DateTime.TryParse(filterValue.ToString(), out temp))
                            {
                                filterDescriptor += String.Format("{0}>='{1}'", id + "[" + filterMember + "]", filterValue);
                            }
                            else
                            {
                                filterDescriptor += String.Format("{0}>={1}", id + "[" + filterMember + "]", filterValue);
                            }

                            break;
                        case FilterOperator.IsGreaterThan:
                            if (DateTime.TryParse(filterValue.ToString(), out temp))
                            {
                                filterDescriptor += String.Format("{0}>'{1}'", id + "[" + filterMember + "]", filterValue);
                            }
                            else
                            {
                                filterDescriptor += String.Format("{0}>{1}", id + "[" + filterMember + "]", filterValue);
                            }

                            break;
                    }
                }
                else
                {
                    filterDescriptor += "1=1";
                }

                filters = filterDescriptor;
            }

            filters = filters.EndsWith("And ") == true ? string.Format("{0})", filters.Substring(0, filters.Length - 4)) : filters;
            filters = filters.EndsWith("Or ") == true ? string.Format("{0})", filters.Substring(0, filters.Length - 4)) : filters;

            return filters;
        }

        public string ApplyFilterNotN(IFilterDescriptor filter, string id)
        {
            var filters = string.Empty;
            if (filter is CompositeFilterDescriptor)
            {
                filters += "(";
                var compositeFilterDescriptor = (CompositeFilterDescriptor)filter;
                foreach (IFilterDescriptor childFilter in compositeFilterDescriptor.FilterDescriptors)
                {
                    filters += ApplyFilterNotN(childFilter, id);
                    filters += string.Format(" {0} ", compositeFilterDescriptor.LogicalOperator.ToString());
                }
            }
            else
            {
                string filterDescriptor = String.Empty;
                var descriptor = (FilterDescriptor)filter;
                var filterMember = descriptor.Member;
                var filterValue = descriptor.Value;

                DateTime temp;

                switch (descriptor.Operator)
                {
                    case FilterOperator.IsEqualTo:
                        filterDescriptor += String.Format("{0} = '{1}'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.IsNotEqualTo:
                        filterDescriptor += String.Format("{0} <> '{1}'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.StartsWith:
                        filterDescriptor += String.Format("{0} like '{1}%'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.Contains:
                        filterDescriptor += String.Format("{0} like '%{1}%'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.EndsWith:
                        filterDescriptor += String.Format("{0} like '%{1}'", id + "[" + filterMember + "]", filterValue);
                        break;
                    case FilterOperator.IsLessThanOrEqualTo:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0} <='{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0} <={1}", id + "[" + filterMember + "]", filterValue);
                        }
                        break;
                    case FilterOperator.IsLessThan:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0}<'{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0}<{1}", id + "[" + filterMember + "]", filterValue);
                        }
                        break;
                    case FilterOperator.IsGreaterThanOrEqualTo:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0}>='{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0}>={1}", id + "[" + filterMember + "]", filterValue);
                        }
                        break;
                    case FilterOperator.IsGreaterThan:
                        if (DateTime.TryParse(filterValue.ToString(), out temp))
                        {
                            filterDescriptor += String.Format("{0}>'{1}'", id + "[" + filterMember + "]", filterValue);
                        }
                        else
                        {
                            filterDescriptor += String.Format("{0}>{1}", id + "[" + filterMember + "]", filterValue);
                        }
                        break;
                }

                filters = filterDescriptor;
            }

            filters = filters.EndsWith("And ") == true ? string.Format("{0})", filters.Substring(0, filters.Length - 4)) : filters;
            filters = filters.EndsWith("Or ") == true ? string.Format("{0})", filters.Substring(0, filters.Length - 4)) : filters;

            return filters;
        }
    }
}