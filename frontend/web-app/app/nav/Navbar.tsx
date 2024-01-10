import React from 'react';
import {IoCarSportSharp} from 'react-icons/io5'
import Search from './Search';
import { useParamsStore } from '@/hooks/useParamsStore';
import Logo from './Logo';

export default function Navbar() {
 
  return (
    <header className='sticky top-0 z-50 flex justify-between bg-white p-5 items-center text-gray-800 shadow-md'>
        <Logo />
        <Search />
        <div>Login</div>
    </header>
  )
}