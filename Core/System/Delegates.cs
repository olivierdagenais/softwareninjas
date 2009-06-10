﻿using System;

namespace System
{
    #region Action
    /// <summary>
    /// Encapsulates a method that takes no parameters and does not return a value.
    /// </summary>
    public delegate void Action();

    /// <summary>
    /// Encapsulates a method that takes a single parameter and does not return a value.
    /// </summary>
    /// 
    /// <typeparam name="T">
    /// The type of the parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="obj">
    /// The parameter of the method that this delegate encapsulates.
    /// </param>
    public delegate void Action<T>(T obj);

    /// <summary>
    /// Encapsulates a method that has two parameters and does not return a value.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T2">
    /// The type of the second parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg1">
    /// The first parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg2">
    /// The second parameter of the method that this delegate encapsulates.
    /// </param>
    public delegate void Action<T1, T2>(T1 arg1, T2 arg2);

    /// <summary>
    /// Encapsulates a method that takes three parameters and does not return a value.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T2">
    /// The type of the second parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T3">
    /// The type of the third parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg1">
    /// The first parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg2">
    /// The second parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg3">
    /// The third parameter of the method that this delegate encapsulates.
    /// </param>
    public delegate void Action<T1, T2, T3>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Encapsulates a method that has four parameters and does not return a value.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T2">
    /// The type of the second parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T3">
    /// The type of the third parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T4">
    /// The type of the fourth parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg1">
    /// The first parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg2">
    /// The second parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg3">
    /// The third parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg4">
    /// The fourth parameter of the method that this delegate encapsulates.
    /// </param>
    public delegate void Action<T1, T2, T3, T4>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    #endregion

    #region Func
    /// <summary>
    /// Encapsulates a method that has no parameters and returns a value of the type specified by the
    /// <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// 
    /// <typeparam name="TResult">
    /// The type of the return value of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <returns>
    /// The return value of the method that this delegate encapsulates.
    /// </returns>
    public delegate TResult Func<TResult>();

    /// <summary>
    /// Encapsulates a method that has one parameter and returns a value of the type specified by the
    /// <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="TResult">
    /// The type of the return value of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg">
    /// The parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <returns>
    /// The return value of the method that this delegate encapsulates.
    /// </returns>
    public delegate TResult Func<T1, TResult>(T1 arg);

    /// <summary>
    /// Encapsulates a method that has two parameters and returns a value of the type specified by the
    /// <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T2">
    /// The type of the second parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="TResult">
    /// The type of the return value of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg1">
    /// The first parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg2">
    /// The second parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <returns>
    /// The return value of the method that this delegate encapsulates.
    /// </returns>
    public delegate TResult Func<T1, T2, TResult>(T1 arg1, T2 arg2);

    /// <summary>
    /// Encapsulates a method that has three parameters and returns a value of the type specified by the
    /// <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T2">
    /// The type of the second parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T3">
    /// The type of the third parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="TResult">
    /// The type of the return value of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg1">
    /// The first parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg2">
    /// The second parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg3">
    /// The third parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <returns>
    /// The return value of the method that this delegate encapsulates.
    /// </returns>
    public delegate TResult Func<T1, T2, T3, TResult>(T1 arg1, T2 arg2, T3 arg3);

    /// <summary>
    /// Encapsulates a method that has four parameters and returns a value of the type specified by the
    /// <typeparamref name="TResult"/> parameter.
    /// </summary>
    /// 
    /// <typeparam name="T1">
    /// The type of the first parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T2">
    /// The type of the second parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T3">
    /// The type of the third parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="T4">
    /// The type of the fourth parameter of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <typeparam name="TResult">
    /// The type of the return value of the method that this delegate encapsulates.
    /// </typeparam>
    /// 
    /// <param name="arg1">
    /// The first parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg2">
    /// The second parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg3">
    /// The third parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <param name="arg4">
    /// The fourth parameter of the method that this delegate encapsulates.
    /// </param>
    /// 
    /// <returns>
    /// The return value of the method that this delegate encapsulates.
    /// </returns>
    public delegate TResult Func<T1, T2, T3, T4, TResult>(T1 arg1, T2 arg2, T3 arg3, T4 arg4);
    #endregion
}
