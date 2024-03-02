using SqlSugar;
using System.Data;
using System.Linq.Expressions;

namespace com.hmdp.repo
{
    public interface IBaseService<TEntity> where TEntity : class
    {
        public ISqlSugarClient Db { get; set; }
        Task<int> Count(Expression<Func<TEntity, bool>> whereString);
        Task<TEntity> QueryById(object objId);
        Task<TEntity> QueryById(object objId, bool blnUseCache = false);
        Task<List<TEntity>> QueryByIDs(object[] lstIds);
        Task<List<TEntity>> QueryByIDsWithExpression(Expression<Func<TEntity, bool>> expression);
        Task<long> Add(TEntity model);
        Task<long> AddNoSnow(TEntity entity);

        Task<List<long>> Add(List<TEntity> listEntity);
        Task<long> AddAndIgnoreColumns(TEntity entity, Expression<Func<TEntity, object>> ignoreColumns);

        Task<bool> DeleteById(object id);

        Task<bool> Delete(TEntity model);
        Task<bool> DeleteNoEntity(Expression<Func<TEntity, bool>> whereExpression);
        
        Task<bool> DeleteByIds(object[] ids);

        Task<bool> Update(TEntity model);
        Task<bool> Update(List<TEntity> model);
        Task<bool> Update(TEntity entity, string where);

        Task<bool> Update(object operateAnonymousObjects);

        Task<bool> Update(TEntity entity, List<string> lstColumns = null, List<string> lstIgnoreColumns = null, string where = "");

        Task<int> Update(Expression<Func<TEntity, bool>> setColumns, Expression<Func<TEntity, bool>> whereColumns, List<string> lstIgnoreColumns = null);
        Task<List<TEntity>> Query();
        Task<List<TEntity>> Query(string where);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, string orderByFields);
        Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression);
        Task<List<TResult>> Query<TResult>(Expression<Func<TEntity, TResult>> expression, Expression<Func<TEntity, bool>> whereExpression, string orderByFields);
        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, Expression<Func<TEntity, object>> orderByExpression, bool isAsc = true);
        Task<List<TEntity>> Query(string where, string orderByFields);
        Task<List<TEntity>> QuerySql(string sql, SugarParameter[] parameters = null);
        Task<DataTable> QueryTable(string sql, SugarParameter[] parameters = null);

        Task<List<TEntity>> Query(Expression<Func<TEntity, bool>> whereExpression, int top, string orderByFields);
        Task<List<TEntity>> Query(string where, int top, string orderByFields);

        Task<List<TEntity>> Query(
            Expression<Func<TEntity, bool>> whereExpression, int pageIndex, int pageSize, string orderByFields);
        Task<List<TEntity>> Query(string where, int pageIndex, int pageSize, string orderByFields);


        Task<PageModel<TEntity>> QueryPage(Expression<Func<TEntity, bool>> whereExpression, int pageIndex = 1, int pageSize = 20, string orderByFields = null);

        Task<List<TResult>> QueryMuch<T, T2, T3, TResult>(
            Expression<Func<T, T2, T3, object[]>> joinExpression,
            Expression<Func<T, T2, T3, TResult>> selectExpression,
            Expression<Func<T, T2, T3, bool>> whereLambda = null) where T : class, new();
        Task<PageModel<TEntity>> QueryPage(PaginationModel pagination);

        #region 分表
        Task<TEntity> QueryByIdSplit(object objId);
        Task<List<long>> AddSplit(TEntity entity);
        Task<bool> DeleteSplit(TEntity entity, DateTime dateTime);
        Task<bool> UpdateSplit(TEntity entity, DateTime dateTime);
        Task<PageModel<TEntity>> QueryPageSplit(Expression<Func<TEntity, bool>> whereExpression, DateTime beginTime, DateTime endTime, int pageIndex = 1, int pageSize = 20, string orderByFields = null);
        #endregion
    }

}
