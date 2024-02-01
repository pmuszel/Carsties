'use server'

import { Auction, PageResult } from "@/types";
import { getTokenWalkaround } from "./authActions";

export async function getData(query: string): Promise<PageResult<Auction>> {
    const url = `http://localhost:6001/search${query}`;

    console.log(url);

    const res = await fetch(url);

    if(!res.ok) throw new Error('Failed to fetch data');

    return res.json();
}

export async function UpdateAuctionTest() {
    const data = {
        milage: Math.floor(Math.random() * 100000) + 1
    }

    const token = await getTokenWalkaround();

    const res = await fetch('http://localhost:6001/auctions/afbee524-5972-4075-8800-7d1f9d7b0a0c', {
        method: 'PUT',
        headers: {
            'Content-Type' : 'application/json',
            'Authorization': 'Bearer ' + token?.access_token
        },
        body: JSON.stringify(data)
    });

    if(!res.ok) return {status: res.status, message: res.statusText};

    return res.statusText;
}